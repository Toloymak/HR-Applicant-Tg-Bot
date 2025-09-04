namespace CandidateTgBot.Types.Callbacks;

public record CancelRevokeApplicationCallback : ICallback, IHasConstantCommandName
{
    public static string CommandName => "keep_app";

    public string Command => CommandName;

    public TgCallbackData ToTgString() => new(CommandName, string.Empty);

    public static CancelRevokeApplicationCallback? Parse(string? data)
    {
        return new CancelRevokeApplicationCallback();
    }

    public string GetDebugString()
        => "CancelRevokeApplicationCallback";
}
