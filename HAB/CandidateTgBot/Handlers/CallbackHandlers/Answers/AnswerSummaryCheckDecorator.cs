using CandidateTgBot.Services;
using CandidateTgBot.Services.DataProviders;
using CandidateTgBot.Types.Callbacks;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers.CallbackHandlers.Answers;

public class AnswerSummaryCheckDecorator<T>
    : ICallbackHandler<T> where T : ICallback
{
    private readonly ICallbackHandler<T> _handler;
    private readonly IProvideUserFromCallback _userProvider;
    private readonly ILogger<AnswerSummaryCheckDecorator<T>> _logger;
    private readonly ApplicationCompleter _applicationCompleter;

    public AnswerSummaryCheckDecorator(
        ICallbackHandler<T> handler,
        IProvideUserFromCallback userProvider,
        ILogger<AnswerSummaryCheckDecorator<T>> logger,
        ApplicationCompleter applicationCompleter)
    {
        _handler = handler;
        _userProvider = userProvider;
        _logger = logger;
        _applicationCompleter = applicationCompleter;
    }

    public async Task Handle(
        long chatId,
        T command,
        CallbackQuery callbackQuery,
        CancellationToken ct)
    {
        await _handler.Handle(chatId, command, callbackQuery, ct);
        
        var botUserId = await _userProvider.GetBotUserId(callbackQuery, ct);
        if (botUserId is null)
            return;
        
        await _applicationCompleter.TryCompleteApplication(botUserId.Value, ct);
    }
}