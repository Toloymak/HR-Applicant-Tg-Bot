namespace CandidateTgBot.Types.Callbacks;

public record ConfirmRevokeApplicationCallback : ICallback, IHasConstantCommandName, IParsableCallback<ConfirmRevokeApplicationCallback>
{
    public static string CommandName => "confirm_revoke";
    public string Command => CommandName;
    
    public Guid ApplicationId { get; init; }
    
    public TgCallbackData ToTgString() => new(CommandName, GuidShort.ToBase64Url(ApplicationId));
    
    public static ConfirmRevokeApplicationCallback? Parse(string? data)
    {
        if (string.IsNullOrEmpty(data)) return null;
        try
        {
            return new ConfirmRevokeApplicationCallback { ApplicationId = GuidShort.FromBase64Url(data) };
        }
        catch (Exception)
        {
            return null;
        }
    }

    public string GetDebugString()
    => "ConfirmRevokeApplicationCallback";
}
