namespace CandidateTgBot.Types.Callbacks;

public record ConfirmCancelApplicationCallback : ICallback, IHasConstantCommandName
{
    public static string CommandName => "confirm_cancel";

    public Guid ApplicationId { get; init; }
    public string Command => CommandName;

    public TgCallbackData ToTgString() => new(CommandName, GuidShort.ToBase64Url(ApplicationId));

    public static ConfirmCancelApplicationCallback? Parse(string? data)
    {
        if (string.IsNullOrEmpty(data)) return null;
        return new ConfirmCancelApplicationCallback { ApplicationId = GuidShort.FromBase64Url(data) };
    }
}
