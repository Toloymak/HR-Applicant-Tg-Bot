using Application.Client.Services;

namespace Application.Services;

public class LogoutService : ILogoutService
{
    public async Task<bool> Logout()
    {
        await Task.Delay(200); // Simulate async operation
        return true;
    }
}