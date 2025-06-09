using Application.Client.Services;

namespace Application.Services;

public class WhoAmI : IWhoAmIService
{
    public Task AuthMe()
    { 
        return Task.CompletedTask;
    }
}