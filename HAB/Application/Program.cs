using System.Text;
using System.Text.Json.Serialization.Metadata;
using ApiCore.Options;
using ApiCore.Services;
using Application;
using Application.Client.Services;
using Application.Client.Services.Bot;
using Application.Client.Services.HrUsers;
using Application.Components;
using Application.Configurations;
using Application.HostedSevices;
using Application.Policies;
using Application.Services;
using Application.UIServices;
using CandateTgBot.Shared.Services;
using CandidateTgBot;
using CandidateTgBot.Handlers;
using CandidateTgBot.Services;
using DataLayer.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Services;
using Telegram.Bot;
using UiShared.Sercvies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    // .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddDbContext<HrBotContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
#if DEBUG
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
#endif
    );

builder.Services.RegisterOptions();
builder.Services.AddMudServices();

ApplicationCompositionRoot.RegisterServices(builder.Services);
CandidateTgBotCompositionRoot.Register(builder.Services);

builder.Services.AddScoped<IProvidePublicBotInfo, ProvidePublicBotInfo>();

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.TypeInfoResolverChain
        .Insert(0, new DefaultJsonTypeInfoResolver());
});

builder.Services.RegisterAuth(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<HrBotContext>();
    await dbContext.Database.MigrateAsync();
}

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