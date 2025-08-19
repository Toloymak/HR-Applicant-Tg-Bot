namespace CandidateTgBot.Types.Callbacks;

public record CancelRevokeCallback : ICallback, IHasConstantCommandName
{
    public static string CommandName => "cancel_revoke";
    public string Command => CommandName;
    
    public TgCallbackData ToTgString() => new(CommandName);
    
    public static CancelRevokeCallback? Parse(string? _) => new();
}
