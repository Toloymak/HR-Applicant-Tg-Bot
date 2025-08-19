using CandateTgBot.Shared.Services;
using CandidateTgBot.Handlers;
using CandidateTgBot.Handlers.Commands;
using CandidateTgBot.Helpers;
using CandidateTgBot.Services;
using CandidateTgBot.Services.CommunicationServices;
using CandidateTgBot.Handlers.CallbackHandlers;
using CandidateTgBot.Types.Callbacks;
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
        service.AddTransient<BotUserService>();
        service.AddTransient<ApplicationService>();
        service.AddTransient<CancelApplicationButtonService>();
        service.AddTransient<BotMessageService>();
        service.AddTransient<WelcomeCommunicationService>();
        service.AddTransient<VacancyListCommunicationService>();
        service.AddTransient<CallbackMessageHandler>();
        service.AddTransient<ButtonCallbackParser>();

        // Callback handlers
        service.AddTransient<ICallbackHandler<VacancyInfoCallback>, VacancyInfoCallbackHandler>();
        service.AddTransient<ICallbackHandler<UpdateVacancyListCallback>, UpdateVacancyListCallbackHandler>();
        service.AddTransient<ICallbackHandler<ShowOtherVacanciesCallback>, ShowOtherVacanciesCallbackHandler>();
        service.AddTransient<ICallbackHandler<ApplyForVacancyCallback>, ApplyForVacancyCallbackHandler>();
        service.AddTransient<ICallbackHandler<CancelApplicationCallback>, CancelApplicationCallbackHandler>();
        service.AddTransient<ICallbackHandler<ConfirmCancelApplicationCallback>, ConfirmCancelApplicationCallbackHandler>();
        service.AddTransient<ICallbackHandler<ConfirmResetCallback>, ConfirmResetCallbackHandler>();
        service.AddTransient<ICallbackHandler<KeepApplicationCallback>, KeepApplicationCallbackHandler>();
        service.AddTransient<ICallbackHandler<KeepProgressCallback>, KeepProgressCallbackHandler>();
    }
}
