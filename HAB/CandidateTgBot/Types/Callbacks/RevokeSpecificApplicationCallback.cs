namespace CandidateTgBot.Types.Callbacks;

public record RevokeSpecificApplicationCallback : ICallback, IHasConstantCommandName
{
    public static string CommandName => "revoke_app";
    public string Command => CommandName;
    
    public Guid ApplicationId { get; init; }
    
    public TgCallbackData ToTgString() => new(CommandName, GuidShort.ToBase64Url(ApplicationId));
    
    public static RevokeSpecificApplicationCallback? Parse(string? data)
    {
        if (string.IsNullOrEmpty(data)) return null;
        try
        {
            return new RevokeSpecificApplicationCallback { ApplicationId = GuidShort.FromBase64Url(data) };
        }
        catch (Exception)
        {
            return null;
        }
    }

    public string GetDebugString()
    => "RevokeSpecificApplicationCallback";
}
