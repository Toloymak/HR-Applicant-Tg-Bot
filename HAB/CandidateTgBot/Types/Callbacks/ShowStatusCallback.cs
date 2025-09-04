namespace CandidateTgBot.Types.Callbacks;

public record ShowStatusCallback : ICallback, IHasConstantCommandName
{
    public static string CommandName => "show_status";
    public string Command => CommandName;
    
    public TgCallbackData ToTgString() => new(CommandName);
    
    public static ShowStatusCallback? Parse(string? _) => new();
    public string GetDebugString()
    => "ShowStatusCallback";
}
