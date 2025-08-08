namespace CandidateTgBot.Types.Callbacks;

public record ShowOtherVacanciesCallback : ICallback, IHasConstantCommandName
{
    public static string CommandName => "so";
    public string Command => CommandName;

    public TgCallbackData ToTgString() => new(CommandName);

    public static ShowOtherVacanciesCallback Parse(string? _) => new();
}

