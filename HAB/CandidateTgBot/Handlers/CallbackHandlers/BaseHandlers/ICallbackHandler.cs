using CandidateTgBot.Types.Callbacks;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers.CallbackHandlers;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface ICallbackHandler<in T> 
    : ICallbackHandler
    where T : ICallback
{
    Task Handle(long chatId, T command, CallbackQuery callbackQuery, CancellationToken ct);
    
    async Task ICallbackHandler.Handle(
        long chatId,
        ICallback command,
        CallbackQuery query,
        CancellationToken ct)
        => await Handle(chatId, (T) command, query, ct);
}

public interface ICallbackHandler
{
    Task Handle(
        long chatId,
        ICallback command,
        CallbackQuery query,
        CancellationToken token);
}