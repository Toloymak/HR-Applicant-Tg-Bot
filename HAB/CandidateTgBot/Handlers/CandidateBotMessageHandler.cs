using System.Reflection;
using CandidateTgBot.Handlers.Commands;
using CandidateTgBot.Helpers;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers;

public class CandidateBotMessageHandler
{
    private readonly ILogger<CandidateBotMessageHandler> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly BotCommandHandler _botCommandHandler;

    public CandidateBotMessageHandler(
        ILogger<CandidateBotMessageHandler> logger,
        IServiceProvider serviceProvider,
        BotCommandHandler botCommandHandler)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _botCommandHandler = botCommandHandler;
    }

    public async Task HandleUpdateAsync(
        ITelegramBotClient bot,
        Update update,
        CancellationToken token)
    {
        var message = update.Message;
        var callback = update.CallbackQuery;
        var chatId = message?.Chat.Id ?? callback?.Message?.Chat.Id;
        
        _logger.LogInformation(
            "Received update: {UpdateType} " +
            "from chat ID: {ChatId}, " +
            "Message: {MessageText}, " +
            "Callback: {CallbackData}",
            update.Type,
            chatId,
            message?.ToString(),
            callback?.Data
        );
        
        if (chatId == null) 
            return;

        if (message == null)
            return;

        if (message.Text != null
            && await _botCommandHandler
                .TryExecuteCommand(message.Text, chatId.Value, token))
            return;

        await bot.SendMessage(
            chatId: chatId,
            text: $"pong ({message.Text})",
            cancellationToken: token
        );

        if (callback != null)
        {
            await bot.SendMessage(
                chatId: chatId,
                text: $"pong ({callback.Data})",
                cancellationToken: token
            );
            
            await bot.AnswerCallbackQuery(
                callbackQueryId: callback.Id,
                text: $"Callback received successfully! {callback} -{callback.Data}-",
                showAlert: false,
                cancellationToken: token
            );
        }
    }

    
}