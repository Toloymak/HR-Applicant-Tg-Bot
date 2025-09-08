namespace CandidateTgBot.Types.Callbacks;

public record AnswerYesCallback : ICallback, IHasConstantCommandName, IParsableCallback<AnswerYesCallback>
{
    public static string CommandName => "answer_yes";
    public string Command => CommandName;
    
    public Guid QuestionId { get; init; }
    
    public TgCallbackData ToTgString() => new(CommandName, GuidShort.ToBase64Url(QuestionId));
    
    public static AnswerYesCallback? Parse(string? data)
    {
        if (string.IsNullOrEmpty(data)) return null;
        try
        {
            return new AnswerYesCallback { QuestionId = GuidShort.FromBase64Url(data) };
        }
        catch (Exception)
        {
            return null;
        }
    }

    public string GetDebugString() => "QuestionId:" + QuestionId;
}

public record AnswerNoCallback : ICallback, IHasConstantCommandName, IParsableCallback<AnswerNoCallback>
{
    public static string CommandName => "answer_no";
    public string Command => CommandName;
    
    public Guid QuestionId { get; init; }
    
    public TgCallbackData ToTgString() => new(CommandName, GuidShort.ToBase64Url(QuestionId));
    
    public static AnswerNoCallback? Parse(string? data)
    {
        if (string.IsNullOrEmpty(data)) return null;
        try
        {
            return new AnswerNoCallback { QuestionId = GuidShort.FromBase64Url(data) };
        }
        catch (Exception)
        {
            return null;
        }
    }

    public string GetDebugString()
        => "QuestionId:" + QuestionId;
}

public record AnswerTextCallback : ICallback, IHasConstantCommandName, IParsableCallback<AnswerTextCallback>
{
    public static string CommandName => "answer_text";
    public string Command => CommandName;
    
    public Guid QuestionId { get; init; }
    
    public TgCallbackData ToTgString() => new(CommandName, GuidShort.ToBase64Url(QuestionId));
    
    public static AnswerTextCallback? Parse(string? data)
    {
        if (string.IsNullOrEmpty(data)) return null;
        try
        {
            return new AnswerTextCallback { QuestionId = GuidShort.FromBase64Url(data) };
        }
        catch (Exception)
        {
            return null;
        }
    }

    public string GetDebugString()
        => "QuestionId:" + QuestionId;
}
