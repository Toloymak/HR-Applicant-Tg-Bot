namespace CandidateTgBot.Types.Callbacks;

public record VacancyInfoCallback 
    : ICallback, IHasConstantCommandName, IParsableCallback<VacancyInfoCallback>
        
{
    public static string CommandName => "vi";

    public required Guid VacancyId { get; init; }
    public string Command => CommandName;
    
    public TgCallbackData ToTgString()
        => new(CommandName, GuidShort.ToBase64Url(VacancyId));

    public static VacancyInfoCallback? Parse(string? data)
    {
        if (string.IsNullOrEmpty(data))
            return null;

        var vacancyId = GuidShort.FromBase64Url(data);
        return new VacancyInfoCallback { VacancyId = vacancyId };
    }

    public string GetDebugString()
    => "VacancyId:" + VacancyId;
}