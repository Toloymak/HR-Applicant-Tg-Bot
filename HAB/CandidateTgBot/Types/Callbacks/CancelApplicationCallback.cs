namespace CandidateTgBot.Types.Callbacks;

public record CancelApplicationCallback : ICallback, IHasConstantCommandName
{
    public static string CommandName => "cancel_app";

    public Guid ApplicationId { get; init; }
    public string Command => CommandName;

    public TgCallbackData ToTgString() => new(CommandName, GuidShort.ToBase64Url(ApplicationId));

    public static CancelApplicationCallback? Parse(string? data)
    {
        if (string.IsNullOrEmpty(data)) return null;
        return new CancelApplicationCallback { ApplicationId = GuidShort.FromBase64Url(data) };
    }

    public string GetDebugString()
    => "ApplicationId:" + ApplicationId;
}
