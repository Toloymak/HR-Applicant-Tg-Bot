using CandidateTgBot.Types.Callbacks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public sealed class CallbackExceptionHandlerDecorator<T>
    : ICallbackHandler<T> where T : ICallback
{
    private readonly ICallbackHandler<T> _handler;
    private readonly ITelegramBotClient _tg;
    private readonly ILogger<CallbackExceptionHandlerDecorator<T>> _logger;

    public CallbackExceptionHandlerDecorator(
        ICallbackHandler<T> handler,
        ITelegramBotClient tg,
        ILogger<CallbackExceptionHandlerDecorator<T>> logger)
    {
        _handler = handler;
        _tg = tg;
        _logger = logger;
    }

    public async Task Handle(
        long chatId,
        T command,
        CallbackQuery callbackQuery,
        CancellationToken ct)
    {
        try
        {
            await _handler.Handle(chatId, command, callbackQuery, ct);
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "Error handling callback {CallbackType} for chat {ChatId}. Details: {CallbackDetails}",
                typeof(T).Name,
                chatId,
                command.GetDebugString());

            await SendErrorMessage(chatId, ct);
        }
    }

    private async Task SendErrorMessage(
        long chatId,
        CancellationToken ct)
    {
        try
        {
            await _tg.SendMessage(
                chatId: chatId,
                text: "❌ An error occurred while processing your answer. Please try again.",
                cancellationToken: ct
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending error message in ExceptionHandlerDecorator");
        }
    }
}