using CandidateTgBot.Services;
using CandidateTgBot.Types.Callbacks;
using DataLayer.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public class CancelApplicationCallbackHandler : ICallbackHandler<CancelApplicationCallback>
{
    private readonly ITelegramBotClient _tg;
    private readonly HrBotContext _context;
    private readonly ApplicationService _applicationService;
    private readonly BotUserService _botUserService;
    private readonly ILogger<CancelApplicationCallbackHandler> _logger;

    public CancelApplicationCallbackHandler(
        ITelegramBotClient tg,
        HrBotContext context,
        ApplicationService applicationService,
        BotUserService botUserService,
        ILogger<CancelApplicationCallbackHandler> logger)
    {
        _tg = tg;
        _context = context;
        _applicationService = applicationService;
        _botUserService = botUserService;
        _logger = logger;
    }

    public async Task Handle(
        long chatId,
        CancelApplicationCallback command,
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

            // Check if application is already canceled or completed
            if (application.State == ApplicationStatus.CanceledByUser)
            {
                await _tg.SendMessage(
                    chatId: chatId,
                    text: "ℹ️ This application has already been canceled.",
                    cancellationToken: ct
                );
                return;
            }

            if (application.State == ApplicationStatus.CompletedByUser ||
                application.State == ApplicationStatus.ApprovedByHr ||
                application.State == ApplicationStatus.RejectedByHr)
            {
                await _tg.SendMessage(
                    chatId: chatId,
                    text: "❌ Cannot cancel this application as it has already been completed or processed.",
                    cancellationToken: ct
                );
                return;
            }

            // Show confirmation dialog
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        text: "✅ Yes, Cancel Application",
                        callbackData: new ConfirmCancelApplicationCallback
                        {
                            ApplicationId = command.ApplicationId
                        }.ToTgString().ToString()
                    )
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        text: "❌ No, Keep Application",
                        callbackData: new KeepApplicationCallback().ToTgString().ToString()
                    )
                }
            });

            await _tg.SendMessage(
                chatId: chatId,
                text: $"🤔 **Confirm Application Cancellation**\n\n" +
                      $"Are you sure you want to cancel your application for **{application.Vacancy?.Title}**?\n\n" +
                      $"⚠️ *This action cannot be undone. You will need to start a new application if you change your mind.*",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct
            );

            _logger.LogInformation(
                "User {TgId} ({Username}) requested to cancel application {ApplicationId} for vacancy '{VacancyTitle}'",
                botUser.TgId, botUser.TgName, application.Id, application.Vacancy?.Title);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error handling cancel application callback for user {ChatId}, application {ApplicationId}",
                chatId, command.ApplicationId);

            await _tg.SendMessage(
                chatId: chatId,
                text: "❌ An error occurred while processing your request. Please try again later.",
                cancellationToken: ct
            );
        }
    }
}
