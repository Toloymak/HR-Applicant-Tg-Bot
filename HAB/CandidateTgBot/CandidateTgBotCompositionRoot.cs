using CandidateTgBot.Handlers;
using CandidateTgBot.Handlers.Commands;
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
    }
}