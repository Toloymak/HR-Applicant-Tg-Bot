using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.Models.Responses;

namespace Application.Client.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _http;
    private ClaimsPrincipal _user = new(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(HttpClient http) => _http = http;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var resp = await _http.GetFromJsonAsync<CurrentUser>("/api/whoami");
            if (resp is null)
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            var identity = new ClaimsIdentity(
                resp.Claims.Select(c => new Claim(c.Key, c.Value)), "jwt");

            _user = new ClaimsPrincipal(identity);
        }
        catch
        {
            _user = new ClaimsPrincipal(new ClaimsIdentity());
        }
        
        var state = new AuthenticationState(_user);
        NotifyAuthenticationStateChanged(Task.FromResult(state));

        return state;
    }
}