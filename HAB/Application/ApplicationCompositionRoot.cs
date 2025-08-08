using ApiCore.Options;
using ApiCore.Services;
using Application.Client.Services;
using Application.Client.Services.HrUsers;
using Application.HostedSevices;
using Application.Services;
using Application.UIServices;
using CandateTgBot.Shared.Services;
using CandidateTgBot.Services;
using Microsoft.Extensions.Options;
using MudBlazor.Services;
using Telegram.Bot;
using UiShared.Sercvies;

namespace Application;

public static class ApplicationCompositionRoot
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddHostedService<CandidateBotHostedService>();
        services.AddHostedService<HrBotHostedService>();
        
        services
            .AddSingleton<ITelegramBotClient>(sp =>
            {
                var options = sp
                    .GetRequiredService<IOptions<CandidateBotOptions>>()
                    .Value;
                return new TelegramBotClient(options.Token);
            });
        services.AddSingleton<TelegramAuthValidator>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IProvideCurrentDateTime, DateTimeProvideCurrent>();

        services.AddSingleton<CandidateBot>();

        services.AddTransient<IVacanciesProvider, VacanciesProvider>();
        services.AddTransient<IHrUserProvider, HrUserProvider>();
        services.AddTransient<IHrUserService, HrUserService>();
        services.AddTransient<ILogoutService, LogoutService>();
        services.AddTransient<HrUserRepository>();
        services.AddTransient<VacanciesRepository>();

        services.AddTransient<IWhoAmIService, WhoAmI>();
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<ICreateVacancy, VacanciesProvider>();
        services.AddTransient<IEditVacancy, VacanciesProvider>();
        services.AddTransient<IProvideAvailablePositions, AvailableVacancyProvider>();
        services.AddTransient<IGetVacancyInfo, GetVacancyInfoMock>();
    }
}