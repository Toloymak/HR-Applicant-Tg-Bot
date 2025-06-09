using Application.Client.Configurations;
using Application.Client.RefitClients;
using Application.Client.Services;
using Application.Client.Services.HrUsers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Refit;
using UiShared.Sercvies;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.ConfigureHttpClients(builder);

builder.Services.AddMudServices();
builder.Services.AddTransient<IVacanciesProvider, VacanciesProvider>();
builder.Services.AddTransient<IHrUserProvider, HrUserProvider>();

builder.Services.AddTransient<CustomAuthenticationStateProvider>();
builder.Services.AddTransient<AuthenticationStateProvider>(
    sp => sp.GetRequiredService<CustomAuthenticationStateProvider>());
builder.Services.AddTransient<IWhoAmIService, WhoAmIService>();
builder.Services.AddTransient<IHrUserService, HrUserService>();
builder.Services.AddTransient<ILogoutService, LogoutService>();

builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();