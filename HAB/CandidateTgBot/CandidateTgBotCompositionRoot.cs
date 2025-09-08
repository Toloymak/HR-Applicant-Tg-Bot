using CandateTgBot.Shared.Services;
using CandidateTgBot.Handlers;
using CandidateTgBot.Handlers.Commands;
using CandidateTgBot.Helpers;
using CandidateTgBot.Services;
using CandidateTgBot.Services.CommunicationServices;
using CandidateTgBot.Services.DataProviders;
using CandidateTgBot.Handlers.CallbackHandlers;
using CandidateTgBot.Types.Callbacks;
using Microsoft.Extensions.DependencyInjection;

namespace CandidateTgBot;

public static class CandidateTgBotCompositionRoot
{
    public static void Register(IServiceCollection services)
    {
        services.RegisterMenuCommands();
        services.AddTransient<CandidateBotMessageHandler>();
        services.AddTransient<CandidateBotErrorHandler>();
        services.AddTransient<BotCommandHandler>();
        services.AddTransient<BotUserService>();
        services.AddTransient<ApplicationService>();
        services.AddTransient<CancelApplicationButtonService>();
        services.AddTransient<CurrentQuestionService>();
        services.AddTransient<AnswerService>();
        services.AddTransient<BotMessageService>();
        services.AddTransient<WelcomeCommunicationService>();
        services.AddTransient<VacancyListCommunicationService>();
        services.AddTransient<CallbackMessageHandler>();
        services.AddTransient<ButtonCallbackParser>();
        services.AddTransient<ISendUnableToIdentifyMessage, ErrorCommunicationService>();
        services.AddTransient<ISendCommandParsingErrorMessage, ErrorCommunicationService>();
        services.AddTransient<ApplyVacancyCommunicationService>();
        
        // Data providers
        services.AddTransient<IProvideUserFromCallback, BotUserIdProvider>();
        services.AddTransient<ApplicationStatusProvider>();
        services.AddTransient<StatusCommunicationService>();
        services.AddTransient<AnswerResultCommunicationService>();

        // Callback handlers
        services.Scan(scan => scan
            .FromAssembliesOf(typeof(ICallbackHandler<>))
            .AddClasses(c => c.AssignableTo(typeof(ICallbackHandler<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Decorate(typeof(ICallbackHandler<>), typeof(CallbackExceptionHandlerDecorator<>));
    }
}
