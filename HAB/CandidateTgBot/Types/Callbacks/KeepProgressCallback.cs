namespace CandidateTgBot.Types.Callbacks;

public record KeepProgressCallback : ICallback, IHasConstantCommandName
{
    public static string CommandName => "keep_progress";

    public string Command => CommandName;

    public TgCallbackData ToTgString() => new(CommandName, string.Empty);

    public static KeepProgressCallback? Parse(string? data)
    {
        return new KeepProgressCallback();
    }
}
