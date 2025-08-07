namespace CandidateTgBot.Handlers.Commands;

public interface IMenuBotCommand
{
    static abstract string Command { get; }
    static abstract string Description { get; }

    Task HandleCommand(
        long chatId,
        CancellationToken ct);
}