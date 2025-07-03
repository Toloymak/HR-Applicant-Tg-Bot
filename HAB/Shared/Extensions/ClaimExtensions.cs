using System.Security.Claims;

namespace Shared;

public static class ClaimExtensions
{
    private static T? GetValue<T>(this ClaimsPrincipal principal, string type)
    {
        var claim = principal.Claims.FirstOrDefault(c => c.Type == type);
        if (claim == null)
            return default;

        return (T)Convert.ChangeType(claim.Value, typeof(T));
    }

    public static string GetTgName(this ClaimsPrincipal principal)
    {
        return principal.GetValue<string>(ClaimTypes.Name) ?? string.Empty;
    }

    public static string GetAlias(this ClaimsPrincipal principal)
        => principal.GetValue<string>("alias") ?? "Unknown";

    public static string GetPosition(this ClaimsPrincipal principal)
        => principal.GetValue<string>("position") ?? "Unknown position";

    public static int? GetHrUserId(this ClaimsPrincipal principal)
        => principal.GetValue<int>("hrUserId");
}