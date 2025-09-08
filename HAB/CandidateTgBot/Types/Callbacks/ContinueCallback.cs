namespace CandidateTgBot.Types.Callbacks;

public class ContinueCallback : ICallback, IParsableCallback<ContinueCallback>, IHasConstantCommandName
{
    public string Command => CommandName;
    
    public static string CommandName => "continue";
    
    public string GetDebugString() => "ContinueCallback";
    
    public TgCallbackData ToTgString() => new(Command);
    
    
    public static ContinueCallback? Parse(string? data) => new();
}