namespace Shared;

public static class StringExtensions
{
    /// Get fist X symbols
    public static string TakeLeft(this string str, int count)
        => str.Length <= count ? str : str[..count];

}