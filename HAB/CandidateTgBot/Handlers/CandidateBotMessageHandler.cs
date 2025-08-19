using System.Diagnostics.CodeAnalysis;
using CandidateTgBot.Handlers.Commands;
using CandidateTgBot.Helpers;
using CandidateTgBot.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers;

public class CandidateBotMessageHandler
{
    private readonly ILogger<CandidateBotMessageHandler> _logger;
    private readonly BotCommandHandler _botCommandHandler;
    private readonly CallbackMessageHandler _callbackMessageHandler;
    private readonly BotUserService _botUserService;
    private readonly ITelegramBotClient _tgClient;
    private readonly CancelApplicationButtonService _cancelButtonService;
    private readonly BotMessageService _botMessageService;
    private readonly AnswerService _answerService;


    public CandidateBotMessageHandler(
        ILogger<CandidateBotMessageHandler> logger,
        BotCommandHandler botCommandHandler,
        ButtonCallbackParser buttonCallbackParser,
        CallbackMessageHandler callbackMessageHandler,
        BotUserService botUserService,
        ITelegramBotClient tgClient,
        CancelApplicationButtonService cancelButtonService,
        BotMessageService botMessageService,
        AnswerService answerService)
    {
        _logger = logger;
        _botCommandHandler = botCommandHandler;
        _callbackMessageHandler = callbackMessageHandler;
        _botUserService = botUserService;
        _tgClient = tgClient;
        _cancelButtonService = cancelButtonService;
        _botMessageService = botMessageService;
        _answerService = answerService;
    }

    public async Task HandleUpdateAsync(
        Update update,
        CancellationToken token)
    {
        if (update.Message is {} message)
            await HandleMessage(token, message);
        else if (update.CallbackQuery is {} callbackQuery)
            await HandleCallback(token, callbackQuery);
        else
            _logger.LogError("Received unsupported update type: {UpdateType}", update.Type);
    }

        private async Task HandleCallback(CancellationToken token,
CallbackQuery callbackQuery)
    {
        var chatId = callbackQuery.Message?.Chat.Id;
        if (chatId is null)
        {
            _logger.LogError("Callback query received without chat ID");
            return;
        }
            
        _logger.LogInformation(
            "Received callback query: {CallbackData} " +
            "from chat ID: {ChatId}",
            callbackQuery.Data,
            chatId
        );

        // Update user activity when they interact with callbacks
        if (callbackQuery.From != null)
        {
            try
            {
                await _botUserService.CreateOrUpdateUserAsync(callbackQuery.From, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Failed to create/update user {TgId} ({Username}) from callback", 
                    callbackQuery.From.Id, callbackQuery.From.Username);
                // Continue processing the callback even if user update fails
            }
        }
            
        await _callbackMessageHandler.HandleCallback(
            chatId.Value, callbackQuery, token);
    }

    private async Task HandleMessage(CancellationToken token,
        Message message)
    {
        var chatId = message.Chat.Id;

        _logger.LogInformation(
            "Received message: {MessageText} " +
            "from chat ID: {ChatId}",
            message.Text,
            chatId
        );

        // Create or update user when they send a message
        Guid? botUserId = null;
        if (message.From != null)
        {
            try
            {
                var botUser = await _botUserService.CreateOrUpdateUserAsync(message.From, token);
                botUserId = botUser.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Failed to create/update user {TgId} ({Username})", 
                    message.From.Id, message.From.Username);
                // Continue processing the message even if user creation fails
            }
        }
            
        if (message is {} msg)
            await HandleMessage(chatId, msg, botUserId, token);
    }

    private async Task HandleMessage(
        long chatId,
        Message message,
        Guid? botUserId,
        CancellationToken token)
    {
        // Handle common commands with user context
        if (message.Text != null)
        {
            var normalizedCommand = message.Text.Trim().ToLowerInvariant();
            
            switch (normalizedCommand)
            {
                case "/start":
                    await _botMessageService.SendStartMessageAsync(chatId, botUserId, token);
                    return;
                case "/help":
                    await _botMessageService.SendHelpMessageAsync(chatId, botUserId, token);
                    return;
                case "/reset":
                    await _botMessageService.SendResetMessageAsync(chatId, botUserId, token);
                    return;
                case "/continue":
                    await _botMessageService.SendContinueMessageAsync(chatId, botUserId, token);
                    return;
                case "/status":
                    await _botMessageService.SendStatusMessageAsync(chatId, botUserId, token);
                    return;
            }

            // Try other command handlers
            if (await _botCommandHandler.TryExecuteCommand(message.Text, chatId, token))
                return;

            // Handle text answers for questions
            if (botUserId.HasValue && !message.Text.StartsWith("/"))
            {
                var success = await _answerService.SaveTextAnswerAsync(
                    chatId, botUserId.Value, message.Text, token);

                if (success)
                {
                    // Answer was processed successfully, no need to send additional message
                    return;
                }
                // If not successful, continue to unknown command message
            }
        }

        // Add cancel button to unknown command response if user has active applications
        var cancelKeyboard = botUserId.HasValue
            ? await _cancelButtonService.CreateCancelButtonKeyboardAsync(botUserId.Value, token)
            : null;

        await _tgClient.SendMessage(
            chatId: chatId,
            text: "🤖 I didn't understand that command. Please use /start to begin, /help to see available commands, or /reset to start over.",
            replyMarkup: cancelKeyboard,
            cancellationToken: token
        );
    }
}