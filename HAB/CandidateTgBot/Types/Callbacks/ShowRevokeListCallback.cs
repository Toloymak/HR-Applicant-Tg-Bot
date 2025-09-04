namespace CandidateTgBot.Types.Callbacks;

public record ShowRevokeListCallback : ICallback, IHasConstantCommandName
{
    public static string CommandName => "revoke_list";
    public string Command => CommandName;
    
    public TgCallbackData ToTgString() => new(CommandName);
    
    public static ShowRevokeListCallback? Parse(string? _) => new();
    public string GetDebugString()
    => "ShowRevokeListCallback";
}
