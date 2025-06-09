using System.Security.Cryptography;
using System.Text;
using Domain.Options;
using Microsoft.Extensions.Options;
using Shared.Models.Requests;

namespace Domain.Services;

public class TelegramAuthValidator
{
    private readonly string _botToken;

    public TelegramAuthValidator(
        IOptions<CandidateBotOptions> options)
    {
        _botToken = options.Value.Token;
    }

    public bool ValidateTelegramAuth(
        TelegramAuthRequests user)
    {
        var receivedHash = user.Hash;
        var dataCheckString = GetDataCheckHash(user);

        var key = SHA256.HashData(Encoding.UTF8.GetBytes(_botToken));

        using var hmac = new HMACSHA256(key);
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataCheckString));
        var calculatedHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

        return receivedHash.ToLower().Equals(calculatedHash);
    }

    private static string GetDataCheckHash(TelegramAuthRequests user)
    {
        var data = GetDataAsDic(user);
        return string.Join("\n", data
            .OrderBy(pair => pair.Key)
            .Select(pair => $"{pair.Key}={pair.Value}"));
    }

    private static SortedDictionary<string, string> GetDataAsDic(TelegramAuthRequests user)
    {
        return new SortedDictionary<string, string>
        {
            { "id", user.Id.ToString() },
            { "first_name", user.First_name },
            { "last_name", user.Last_name },
            { "username", user.Username },
            { "photo_url", user.Photo_url },
            { "auth_date", user.Auth_date.ToString() }
        };
    }
}