namespace Application.Client.Services;

public interface ILogoutService
{
    Task<bool> Logout();
}

internal class LogoutService : ILogoutService
{
    private readonly HttpClient _httpClient;

    public LogoutService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> Logout()
    {
        var result = await _httpClient.GetAsync("api/auth/logout");
        return result.IsSuccessStatusCode;
    }
}