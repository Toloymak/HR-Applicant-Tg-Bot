namespace CandidateTgBot.Types.Callbacks;

public record KeepApplicationCallback : ICallback, IHasConstantCommandName
{
    public static string CommandName => "keep_app";

    public string Command => CommandName;

    public TgCallbackData ToTgString() => new(CommandName, string.Empty);

    public static KeepApplicationCallback? Parse(string? data)
    {
        return new KeepApplicationCallback();
    }
}
