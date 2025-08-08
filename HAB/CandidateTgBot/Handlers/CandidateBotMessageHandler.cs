using System.Reflection;
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
    private readonly ButtonCallbackParser _buttonCallbackParser;

    public CandidateBotMessageHandler(
        ILogger<CandidateBotMessageHandler> logger,
        BotCommandHandler botCommandHandler,
        ButtonCallbackParser buttonCallbackParser)
    {
        _logger = logger;
        _botCommandHandler = botCommandHandler;
        _buttonCallbackParser = buttonCallbackParser;
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

        if (message != null)
        {
            if (message.Text != null
                && await _botCommandHandler
                    .TryExecuteCommand(message.Text, chatId.Value, token))
                return;

            await bot.SendMessage(
                chatId: chatId,
                text: $"pong ({message.Text})",
                cancellationToken: token
            );
        }

        if (callback is { Data: {} callbackData } )
        {
            if (_buttonCallbackParser.ParseCallback(callbackData) is { } command)
            {
                await bot.SendMessage(
                    chatId: chatId,
                    text: $"Command received: {command.Command}, command: {command}",
                    cancellationToken: token
                );
                
                await bot.AnswerCallbackQuery(
                    callbackQueryId: callback.Id,
                    text: $"Callback received successfully! {callback} -{callback.Data}-",
                    showAlert: false,
                    cancellationToken: token
                );
            }
            else
            {
                await bot.SendMessage(
                    chatId: chatId,
                    text: $"Error handling command ({callback.Data})",
                    cancellationToken: token
                );
                
                await bot.AnswerCallbackQuery(
                    callbackQueryId: callback.Id,
                    text: $"Error handling command ({callback.Data})",
                    showAlert: false,
                    cancellationToken: token
                );
            }
            
            
        }
    }
}