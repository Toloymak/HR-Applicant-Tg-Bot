namespace CandidateTgBot.Helpers;

public static class CommandHelper
{
    public static string NormalizeCommandToText(string text)
        => NormalizeCommand(text).TrimStart('/');

    public static string NormalizeCommand(string text)
        => text.Trim().Replace("\n", " ").Replace("\r", " ");

    public static bool LooksLikeCommand(string text)
        => text.StartsWith("/", StringComparison.OrdinalIgnoreCase);
}