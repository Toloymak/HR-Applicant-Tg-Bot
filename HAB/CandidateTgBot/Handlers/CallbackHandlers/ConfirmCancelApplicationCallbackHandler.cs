using CandidateTgBot.Services;
using CandidateTgBot.Services.CommunicationServices;
using CandidateTgBot.Types.Callbacks;
using DataLayer.Contexts;
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
    private readonly BotUserService _botUserService;
    private readonly VacancyListCommunicationService _vacancyListService;
    private readonly ILogger<ConfirmCancelApplicationCallbackHandler> _logger;

    public ConfirmCancelApplicationCallbackHandler(
        ITelegramBotClient tg,
        HrBotContext context,
        ApplicationService applicationService,
        BotUserService botUserService,
        VacancyListCommunicationService vacancyListService,
        ILogger<ConfirmCancelApplicationCallbackHandler> logger)
    {
        _tg = tg;
        _context = context;
        _applicationService = applicationService;
        _botUserService = botUserService;
        _vacancyListService = vacancyListService;
        _logger = logger;
    }

    public async Task Handle(
        long chatId,
        ConfirmCancelApplicationCallback command,
        CallbackQuery query,
        CancellationToken ct)
    {
        try
        {
            // Ensure we have the user information
            if (query.From == null)
            {
                await _tg.SendMessage(
                    chatId: chatId,
                    text: "❌ Unable to identify user. Please try again.",
                    cancellationToken: ct
                );
                return;
            }

            // Get the bot user
            var botUser = await _botUserService.CreateOrUpdateUserAsync(query.From, ct);

            // Get the application with vacancy information
            var application = await _context.UserApplications
                .Include(a => a.Vacancy)
                .FirstOrDefaultAsync(a => a.Id == command.ApplicationId && a.BotUserId == botUser.Id, ct);

            if (application == null)
            {
                await _tg.SendMessage(
                    chatId: chatId,
                    text: "❌ Application not found or you don't have permission to cancel it.",
                    cancellationToken: ct
                );
                return;
            }

            // Check if application is already canceled
            if (application.State == ApplicationStatus.CanceledByUser)
            {
                await _tg.SendMessage(
                    chatId: chatId,
                    text: "ℹ️ This application has already been canceled.",
                    cancellationToken: ct
                );
                return;
            }

            // Cancel the application
            await _applicationService.UpdateApplicationStatusAsync(
                command.ApplicationId,
                ApplicationStatus.CanceledByUser,
                ct
            );

            // Send confirmation message
            await _tg.SendMessage(
                chatId: chatId,
                text: $"✅ **Application Canceled Successfully**\n\n" +
                      $"Your application for **{application.Vacancy?.Title}** has been canceled.\n\n" +
                      $"Here are the available positions:",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                cancellationToken: ct
            );

            _logger.LogInformation(
                "User {TgId} ({Username}) successfully canceled application {ApplicationId} for vacancy '{VacancyTitle}'",
                botUser.TgId, botUser.TgName, application.Id, application.Vacancy?.Title);

            // Show vacancy list after successful cancellation
            await _vacancyListService.SendVacancyListAsync(chatId, botUser.Id, ct);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error confirming cancel application for user {ChatId}, application {ApplicationId}",
                chatId, command.ApplicationId);

            await _tg.SendMessage(
                chatId: chatId,
                text: "❌ An error occurred while canceling your application. Please try again later.",
                cancellationToken: ct
            );
        }
    }
}
