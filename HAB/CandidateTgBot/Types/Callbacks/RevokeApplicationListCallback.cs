namespace CandidateTgBot.Types.Callbacks;

public record RevokeApplicationListCallback : ICallback, IHasConstantCommandName, IParsableCallback<RevokeApplicationListCallback>
{
    public static string CommandName => "revoke_list";
    public string Command => CommandName;
    
    public TgCallbackData ToTgString() => new(CommandName);
    
    public static RevokeApplicationListCallback? Parse(string? _) => new();
    public string GetDebugString()
    => "RevokeApplicationListCallback";
}
