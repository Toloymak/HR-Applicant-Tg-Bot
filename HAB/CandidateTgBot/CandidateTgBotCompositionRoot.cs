using CandidateTgBot.Handlers;
using CandidateTgBot.Handlers.Commands;
using CandidateTgBot.Helpers;
using CandidateTgBot.Services;
using CandidateTgBot.Services.CommunicationServices;
using Microsoft.Extensions.DependencyInjection;

namespace CandidateTgBot;

public static class CandidateTgBotCompositionRoot
{
    public static void Register(IServiceCollection service)
    {
        service.RegisterMenuCommands();
        service.AddTransient<CandidateBotMessageHandler>();
        service.AddTransient<CandidateBotErrorHandler>();
        service.AddTransient<BotCommandHandler>();
        service.AddTransient<WelcomeMessageService>();
        service.AddTransient<IProvideAvailablePositions, ProvideAvailablePositionsMock>();
        service.AddTransient<ButtonCallbackParser>();
    }
}