using CandidateTgBot.Handlers.CallbackHandlers;
using CandidateTgBot.Helpers;
using CandidateTgBot.Services.CommunicationServices;
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
    private readonly ISendCommandParsingErrorMessage _sendCommandParsingErrorMessage;

    public CallbackMessageHandler(
        ITelegramBotClient tgClient,
        ButtonCallbackParser buttonCallbackParser,
        IServiceProvider serviceProvider,
        ILogger<CallbackMessageHandler> logger,
        ISendCommandParsingErrorMessage sendCommandParsingErrorMessage)
    {
        _tgClient = tgClient;
        _buttonCallbackParser = buttonCallbackParser;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _sendCommandParsingErrorMessage = sendCommandParsingErrorMessage;
    }

    public async Task HandleCallback(
        long chatId,
        CallbackQuery callback,
        CancellationToken token)
    {
        // Handle special dismiss callback
        if (callback.Data == "dismiss")
        {
            await _tgClient.AnswerCallbackQuery(
                callbackQueryId: callback.Id,
                text: "Action canceled",
                cancellationToken: token
            );
            return;
        }

        if (_buttonCallbackParser.ParseCallback(callback) is { } command)
        {
            if (await TryDispatchToHandler(chatId, command, callback, token))
                return;
        }

        await _sendCommandParsingErrorMessage.Send(chatId, callback, token);
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
