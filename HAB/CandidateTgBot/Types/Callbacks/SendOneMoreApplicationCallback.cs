namespace CandidateTgBot.Types.Callbacks;

public record SendOneMoreApplicationCallback : ICallback, IHasConstantCommandName
{
    public static string CommandName => "send_one_more";
    public string Command => CommandName;
    
    public Guid CompletedApplicationId { get; init; }
    
    public TgCallbackData ToTgString() => new(CommandName, GuidShort.ToBase64Url(CompletedApplicationId));
    
    public static SendOneMoreApplicationCallback? Parse(string? data)
    {
        if (string.IsNullOrEmpty(data)) return null;
        try
        {
            return new SendOneMoreApplicationCallback { CompletedApplicationId = GuidShort.FromBase64Url(data) };
        }
        catch (Exception)
        {
            return null;
        }
    }
}
