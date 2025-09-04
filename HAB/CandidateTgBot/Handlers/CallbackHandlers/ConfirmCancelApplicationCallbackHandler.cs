using CandidateTgBot.Services;
using CandidateTgBot.Services.CommunicationServices;
using CandidateTgBot.Services.DataProviders;
using CandidateTgBot.Types.Callbacks;
using DataLayer.Contexts;
using DataLayer.Dals;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public class ConfirmCancelApplicationCallbackHandler : ICallbackHandler<ConfirmCancelApplicationCallback>
{
    private readonly ITelegramBotClient _tg;
    private readonly HrBotContext _context;
    private readonly ApplicationService _applicationService;
    private readonly VacancyListCommunicationService _vacancyListService;
    private readonly ILogger<ConfirmCancelApplicationCallbackHandler> _logger;
    private readonly ISendUnableToIdentifyMessage _sendUnableToIdentifyMessage;
    private readonly IProvideUserFromCallback _userProvider;

    public ConfirmCancelApplicationCallbackHandler(
        ITelegramBotClient tg,
        HrBotContext context,
        ApplicationService applicationService,
        BotUserService botUserService,
        VacancyListCommunicationService vacancyListService,
        ILogger<ConfirmCancelApplicationCallbackHandler> logger,
        ISendUnableToIdentifyMessage sendUnableToIdentifyMessage,
        IProvideUserFromCallback userProvider)
    {
        _tg = tg;
        _context = context;
        _applicationService = applicationService;
        _vacancyListService = vacancyListService;
        _logger = logger;
        _sendUnableToIdentifyMessage = sendUnableToIdentifyMessage;
        _userProvider = userProvider;
    }

    public async Task Handle(
        long chatId,
        ConfirmCancelApplicationCallback command,
        CallbackQuery query,
        CancellationToken ct)
    {
            var botUser = await _userProvider.GetBotUser(query, ct);
            if (botUser is null)
            {
                await _sendUnableToIdentifyMessage.Send(chatId, ct);
                return;
            }

            var application = await _context.UserApplications
                .Include(a => a.Vacancy)
                .Where(x => x.State == ApplicationStatus.InProgress)
                .FirstOrDefaultAsync(a => a.Id == command.ApplicationId && a.BotUserId == botUser.Id, ct);

            if (application == null)
            {
                await _tg.SendMessage(
                    chatId: chatId,
                    text: "❌ Application not found or you don't have permission to cancel it",
                    cancellationToken: ct
                );
                return;
            }

            await _applicationService.UpdateApplicationStatus(
                command.ApplicationId, ApplicationStatus.Canceled, ct);

            await SendApplicationCancellationConfirmation(chatId, ct, application);

            _logger.LogInformation(
                "User {TgId} ({Username}) successfully canceled application {ApplicationId} for vacancy '{VacancyTitle}'",
                botUser.TgId, botUser.TgName, application.Id, application.Vacancy?.Title);

            await _vacancyListService.SendVacancyListAsync(chatId, botUser.Id, ct);
        
    }

    private async Task SendApplicationCancellationConfirmation(
        long chatId,
        CancellationToken ct,
        UserApplicationDal application)
    {
        await _tg.SendMessage(
            chatId: chatId,
            text: $"✅ **Application Canceled Successfully**\n\n" +
                  $"Your application for **{application.Vacancy?.Title}** has been canceled.\n\n" +
                  $"Here are the available positions:",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            cancellationToken: ct
        );
    }
}
