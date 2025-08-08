using CandidateTgBot.Handlers.CallbackHandlers;
using CandidateTgBot.Helpers;
using CandidateTgBot.Types.Callbacks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers;

public class CallbackMessageHandler
{
    private readonly ITelegramBotClient _tgClient;
    private readonly ButtonCallbackParser _buttonCallbackParser;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CallbackMessageHandler> _logger;

    public CallbackMessageHandler(
        ITelegramBotClient tgClient,
        ButtonCallbackParser buttonCallbackParser,
        IServiceProvider serviceProvider,
        ILogger<CallbackMessageHandler> logger)
    {
        _tgClient = tgClient;
        _buttonCallbackParser = buttonCallbackParser;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task HandleCallback(
        long chatId,
        CallbackQuery callback,
        CancellationToken token)
    {
        if (_buttonCallbackParser.ParseCallback(callback) is { } command)
        {
            if (await TryDispatchToHandler(chatId, command, callback, token))
                return;
        }

        await _tgClient.SendMessage(
            chatId: chatId,
            text: $"Error handling command ({callback.Data})",
            cancellationToken: token
        );
                
        await _tgClient.AnswerCallbackQuery(
            callbackQueryId: callback.Id,
            text: "Sorry, I couldn't parse the command, contact support",
            showAlert: true,
            cancellationToken: token
        );
    }

    private async Task<bool> TryDispatchToHandler(
        long chatId,
        ICallback command,
        CallbackQuery query,
        CancellationToken token)
    {
        var handlerType = typeof(ICallbackHandler<>)
            .MakeGenericType(command.GetType());

        if (_serviceProvider.GetService(handlerType) is not ICallbackHandler handler)
        {
            _logger.LogError("Handler of type {HandlerType} could not be found",
                handlerType);
            return false;
        }
        
        await handler.Handle(chatId, command, query, token);
        
        await _tgClient.AnswerCallbackQuery(
            callbackQueryId: query.Id,
            text: "Processed",
            showAlert: false,
            cancellationToken: token
        );
        
        return true;
    }
}
