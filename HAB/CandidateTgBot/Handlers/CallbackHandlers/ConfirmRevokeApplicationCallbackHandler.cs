using CandidateTgBot.Services;
using CandidateTgBot.Types.Callbacks;
using DataLayer.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public class ConfirmRevokeApplicationCallbackHandler : ICallbackHandler<ConfirmRevokeApplicationCallback>
{
    private readonly ITelegramBotClient _tg;
    private readonly BotUserService _botUserService;
    private readonly ApplicationService _applicationService;
    private readonly ApplicationStatusService _statusService;
    private readonly HrBotContext _context;
    private readonly ILogger<ConfirmRevokeApplicationCallbackHandler> _logger;

    public ConfirmRevokeApplicationCallbackHandler(
        ITelegramBotClient tg,
        BotUserService botUserService,
        ApplicationService applicationService,
        ApplicationStatusService statusService,
        HrBotContext context,
        ILogger<ConfirmRevokeApplicationCallbackHandler> logger)
    {
        _tg = tg;
        _botUserService = botUserService;
        _applicationService = applicationService;
        _statusService = statusService;
        _context = context;
        _logger = logger;
    }

    public async Task Handle(
        long chatId,
        ConfirmRevokeApplicationCallback command,
        CallbackQuery query,
        CancellationToken ct)
    {
        try
        {
            // Get bot user ID from the callback query
            var botUserId = await GetBotUserIdFromCallback(query, ct);
            if (botUserId == null)
            {
                await _tg.SendMessage(
                    chatId: chatId,
                    text: "❌ Unable to identify your user account. Please try again.",
                    cancellationToken: ct
                );
                return;
            }

            // Get the application to revoke
            var application = await _context.UserApplications
                .Include(a => a.Vacancy)
                .FirstOrDefaultAsync(a => a.Id == command.ApplicationId && 
                                         a.BotUserId == botUserId.Value &&
                                         (a.State == ApplicationStatus.CompletedByUser ||
                                          a.State == ApplicationStatus.CompetedByUserAndStartedNew), ct);

            if (application == null)
            {
                await _tg.SendMessage(
                    chatId: chatId,
                    text: "❌ **Application Not Found**\n\n" +
                          "The application you're trying to revoke could not be found or is not eligible for revocation.\n\n" +
                          "💡 *Only applications with 'Submitted for Review' or 'Review by HR' status can be revoked.*",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    cancellationToken: ct
                );
                return;
            }

            // Update application status to RevokedByUser
            await _applicationService.UpdateApplicationStatusAsync(
                application.Id, 
                ApplicationStatus.RevokedByUser, 
                ct);

            // Send success message
            await _tg.SendMessage(
                chatId: chatId,
                text: $"🗑️ **Application Revoked Successfully**\n\n" +
                      $"Your application for **{application.Vacancy?.Title}** has been revoked.\n\n" +
                      $"✅ The application has been removed from your active applications.",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                cancellationToken: ct
            );

            await _tg.AnswerCallbackQuery(
                callbackQueryId: query.Id,
                text: "Application revoked successfully!",
                showAlert: false,
                cancellationToken: ct
            );

            _logger.LogInformation(
                "User {BotUserId} revoked application {ApplicationId} for vacancy '{VacancyTitle}' in chat {ChatId}",
                botUserId, application.Id, application.Vacancy?.Title, chatId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling ConfirmRevokeApplicationCallback for application {ApplicationId}", 
                command.ApplicationId);
            
            await _tg.SendMessage(
                chatId: chatId,
                text: "❌ An error occurred while revoking your application. Please try again.",
                cancellationToken: ct
            );
        }
    }

    private async Task<Guid?> GetBotUserIdFromCallback(CallbackQuery query, CancellationToken ct)
    {
        if (query.From == null) return null;
        
        try
        {
            var botUser = await _botUserService.CreateOrUpdateUserAsync(query.From, ct);
            return botUser?.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bot user for Telegram user {TgId}", query.From.Id);
            return null;
        }
    }
}
