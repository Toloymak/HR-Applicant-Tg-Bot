namespace CandidateTgBot.Types.Callbacks;

public record ApplyForVacancyCallback : ICallback, IHasConstantCommandName
{
    public static string CommandName => "ap";

    public Guid VacancyId { get; init; }
    public string Command => CommandName;

    public TgCallbackData ToTgString() => new(CommandName, GuidShort.ToBase64Url(VacancyId));

    public static ApplyForVacancyCallback? Parse(string? data)
    {
        if (string.IsNullOrEmpty(data)) return null;
        return new ApplyForVacancyCallback { VacancyId = GuidShort.FromBase64Url(data) };
    }

    public string GetDebugString()
    => "VacancyId:" + VacancyId;
}

