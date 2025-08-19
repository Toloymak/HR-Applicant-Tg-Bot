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


    public CandidateBotMessageHandler(
        ILogger<CandidateBotMessageHandler> logger,
        BotCommandHandler botCommandHandler,
        ButtonCallbackParser buttonCallbackParser,
        CallbackMessageHandler callbackMessageHandler,
        BotUserService botUserService,
        ITelegramBotClient tgClient)
    {
        _logger = logger;
        _botCommandHandler = botCommandHandler;
        _callbackMessageHandler = callbackMessageHandler;
        _botUserService = botUserService;
        _tgClient = tgClient;
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
        if (message.From != null)
        {
            try
            {
                await _botUserService.CreateOrUpdateUserAsync(message.From, token);
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
            await HandleMessage(chatId, msg, token);
    }

    private async Task HandleMessage(
        long chatId,
        Message message,
        CancellationToken token)
    {
        if (message.Text != null
            && await _botCommandHandler
                .TryExecuteCommand(message.Text, chatId, token))
            return;

        await _tgClient.SendMessage(
            chatId: chatId,
            text: "🤖 I didn't understand that command. Please use /start to begin, /help to see available commands, or /reset to start over.",
            cancellationToken: token
        );
    }
}