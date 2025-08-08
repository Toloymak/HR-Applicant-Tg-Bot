namespace CandidateTgBot.Types.Callbacks;

public record UpdateVacancyListCallback
    : ICallback, IHasConstantCommandName
{
    public static string CommandName => "ul";
    
    public string Command => CommandName;

    public TgCallbackData ToTgString() => new (CommandName);

    public static UpdateVacancyListCallback Parse(string? _) => new();
}