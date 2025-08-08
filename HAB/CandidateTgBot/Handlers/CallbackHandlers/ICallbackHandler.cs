using CandidateTgBot.Types.Callbacks;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public interface ICallbackHandler<in T> 
    : ICallbackHandler
    where T : ICallback
{
    Task Handle(long chatId, T command, CancellationToken ct);
    
    async Task ICallbackHandler.Handle(
        long chatId, ICallback command, CancellationToken ct)
        => await Handle(chatId, (T) command, ct);
}

public interface ICallbackHandler
{
    Task Handle(long chatId, ICallback command, CancellationToken token);
}