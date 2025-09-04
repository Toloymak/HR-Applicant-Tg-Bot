namespace CandidateTgBot.Types.Callbacks;

public record ConfirmResetCallback : ICallback, IHasConstantCommandName
{
    public static string CommandName => "confirm_reset";

    public string Command => CommandName;

    public TgCallbackData ToTgString() => new(CommandName, string.Empty);

    public static ConfirmResetCallback? Parse(string? data)
    {
        return new ConfirmResetCallback();
    }

    public string GetDebugString()
    => "ConfirmResetCallback";
}
