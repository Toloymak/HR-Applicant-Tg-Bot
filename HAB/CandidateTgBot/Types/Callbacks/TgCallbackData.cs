namespace CandidateTgBot.Types.Callbacks;

public record TgCallbackData(string Command, string? CallbackData = null)
{
    private const char Separator = '|';

    public static TgCallbackData? Parse(string callback)
    {
        var sepIndex = callback.IndexOf(Separator);
        if (sepIndex < 0)
            return null;

        var command = callback[..sepIndex];
        var data = callback[(sepIndex + 1)..];
        
        return new TgCallbackData(command, data);
    }

    public override string ToString()
        => $"{Command}{Separator}{CallbackData ?? string.Empty}";
    
    public static explicit operator string(TgCallbackData callbackData)
        => callbackData.ToString();
}