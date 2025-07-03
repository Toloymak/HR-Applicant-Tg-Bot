using Application.Client.Services;
using Shared.Models.Requests;

namespace Application.Services;

public class AuthService : IAuthService
{
    public Task<bool> AuthOnServer(TelegramAuthRequests request)
    {
        return Task.FromResult(false);
    }
}