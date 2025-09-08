namespace CandidateTgBot.Types.Callbacks;

public record ShowOtherVacanciesCallback : ICallback, IHasConstantCommandName, IParsableCallback<ShowOtherVacanciesCallback>
{
    public static string CommandName => "so";
    public string Command => CommandName;

    public TgCallbackData ToTgString() => new(CommandName);

    public static ShowOtherVacanciesCallback Parse(string? _) => new();
    public string GetDebugString()
    => "ShowOtherVacanciesCallback";
}

