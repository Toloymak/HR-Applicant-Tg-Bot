namespace CandidateTgBot.Types;

public class GuidShort
{
    // Guid -> base64url (22 chars)
    public static string ToBase64Url(Guid guid)
    {
        var bytes = guid.ToByteArray(); // 16 bytes
        var b64 = Convert.ToBase64String(bytes);
        return b64.TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    // base64url (22 chars) -> Guid
    public static Guid FromBase64Url(string s)
    {
        // Back to correct base64
        var b64 = s.Replace('-', '+').Replace('_', '/');
        // восстановить паддинг
        switch (b64.Length % 4)
        {
            case 2: b64 += "=="; break;
            case 3: b64 += "="; break;
        }
        var bytes = Convert.FromBase64String(b64);
        return new Guid(bytes);
    }
}