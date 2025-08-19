namespace CandidateTgBot.Types.Callbacks;

public record StartNewApplicationCallback : ICallback, IHasConstantCommandName
{
    public static string CommandName => "start_new_application";
    public string Command => CommandName;
    
    public TgCallbackData ToTgString() => new(CommandName);
    
    public static StartNewApplicationCallback? Parse(string? _) => new();
}
