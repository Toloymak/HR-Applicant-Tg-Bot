namespace CandidateTgBot.Types.Callbacks;

public record ShowRevokeListCallback : ICallback, IHasConstantCommandName, IParsableCallback<ShowRevokeListCallback>
{
    public static string CommandName => "shw_rvk_lst";
    public string Command => CommandName;
    
    public TgCallbackData ToTgString() => new(CommandName);

    public static ShowRevokeListCallback? Parse(
        string? _) => new();

    public string GetDebugString()
        => "ShowRevokeListCallback";
}
