using System.Text;
using Application.Client.Services;
using Application.Client.Services.HrUsers;
using Application.Components;
using Application.Configurations;
using Application.HostedSevices;
using Application.Services;
using Application.UIServices;
using CandidateTgBot;
using CandidateTgBot.Services;
using DataLayer.Contexts;
using Domain.Options;
using Domain.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Services;
using Telegram.Bot;
using UiShared.Sercvies;
using UKG.HCM.Application.Services;
using UKG.HCM.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    // .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddHostedService<CandidateBotHostedService>();
builder.Services.AddHostedService<HrBotHostedService>();

builder.Services.AddDbContext<HrBotContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services
    .AddOptions<CandidateBotOptions>()
    .Bind(builder.Configuration.GetSection("CandidateBot"))
    .ValidateOnStart();
builder.Services
    .AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection("Jwt"))
    .ValidateOnStart();

// builder.Services.AddHttpClient("https://concrete-mammoth-noticeably.ngrok-free.app/");


builder.Services
    .AddSingleton<ITelegramBotClient>(sp =>
    {
        var options = sp.GetRequiredService<IOptions<CandidateBotOptions>>().Value;
        return new TelegramBotClient(options.Token);
    });
builder.Services.AddSingleton<TelegramAuthValidator>();
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddSingleton<IProvideCurrentDateTime, DateTimeProvideCurrent>();

builder.Services.AddSingleton<CandidateBotService>();
builder.Services.AddMudServices();

builder.Services.AddTransient<IVacanciesProvider, VacanciesProvider>();
builder.Services.AddTransient<IHrUserProvider, HrUserProvider>();
builder.Services.AddTransient<IHrUserService, HrUserService>();
builder.Services.AddTransient<ILogoutService, LogoutService>();


builder.Services.AddTransient<IWhoAmIService, WhoAmI>();

var jwt = builder.Configuration.GetSection("Jwt").GetSection("SigningKey").Value
    ?? throw new InvalidOperationException("JWT Signing Key is not configured.");

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["auth"];
                if (!string.IsNullOrEmpty(token))
                    context.Token = token;
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseAuthentication();
app.UseAuthorization();

app.RegisterAllEndpoints();

app.UseAntiforgery();

app.MapStaticAssets();
app
    .MapRazorComponents<App>()
    // .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Application.Client._Imports).Assembly);

app.Run();