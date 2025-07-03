using System.Net.Http.Json;
using Shared.Models.Requests;

namespace Application.Client.Services;

public interface IAuthService
{
    Task<bool> AuthOnServer(TelegramAuthRequests request);
}

internal class AuthService : IAuthService
{
    private readonly HttpClient _http;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        ILogger<AuthService> logger,
        HttpClient http)
    {
        _logger = logger;
        _http = http;
    }

    public async Task<bool> AuthOnServer(TelegramAuthRequests request)
    {
        var res = await _http.PostAsJsonAsync("api/auth/tg", request);
        
        if (!res.IsSuccessStatusCode)
        {
            _logger.LogError("Auth failed with status code: {StatusCode}", res.StatusCode);
            return false;
        }

        return true;
    }
}