namespace Application.Client.Services;

public interface IWhoAmIService
{
    Task AuthMe();
}

internal class WhoAmIService : IWhoAmIService
{
    private readonly CustomAuthenticationStateProvider _authProvider;

    public WhoAmIService(CustomAuthenticationStateProvider authProvider)
    {
        _authProvider = authProvider;
    }

    public async Task AuthMe()
    {
        await _authProvider.GetAuthenticationStateAsync();
    }
}