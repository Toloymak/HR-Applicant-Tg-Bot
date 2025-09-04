using CandidateTgBot.Extensions;
using CandidateTgBot.Services;
using CandidateTgBot.Services.CommunicationServices;
using CandidateTgBot.Services.DataProviders;
using CandidateTgBot.Types.Callbacks;
using DataLayer.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public class RevokeSpecificApplicationCallbackHandler : ICallbackHandler<RevokeSpecificApplicationCallback>
{
    private readonly ITelegramBotClient _tg;
    private readonly HrBotContext _context;
    private readonly ILogger<RevokeSpecificApplicationCallbackHandler> _logger;
    private readonly ISendUnableToIdentifyMessage _sendUnableToIdentifyMessage;
    private readonly IProvideUserFromCallback _userProvider;

    public RevokeSpecificApplicationCallbackHandler(
        ITelegramBotClient tg,
        BotUserService botUserService,
        HrBotContext context,
        ILogger<RevokeSpecificApplicationCallbackHandler> logger,
        ISendUnableToIdentifyMessage sendUnableToIdentifyMessage,
        IProvideUserFromCallback userProvider)
    {
        _tg = tg;
        _context = context;
        _logger = logger;
        _sendUnableToIdentifyMessage = sendUnableToIdentifyMessage;
        _userProvider = userProvider;
    }

    public async Task Handle(
        long chatId,
        RevokeSpecificApplicationCallback command,
        CallbackQuery query,
        CancellationToken ct)
    {
        try
        {
            var botUserId = await _userProvider.GetBotUserId(query, ct);
            if (botUserId is null)
            {
                await _sendUnableToIdentifyMessage.Send(chatId, ct);
                return;
            }

            // Get the application to revoke
            var application = await _context.UserApplications
                .Include(a => a.Vacancy)
                .FirstOrDefaultAsync(a => a.Id == command.ApplicationId && 
                                         a.BotUserId == botUserId.Value &&
                                         a.State == ApplicationStatus.ReviewByHr, ct);

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

            // Create confirmation keyboard
            var keyboard = new InlineKeyboardMarkup([
                [
                    InlineKeyboardButton.WithCallbackData(
                        text: "✅ Yes, Revoke Application",
                        callbackData: new ConfirmRevokeApplicationCallback { ApplicationId = application.Id }
                            .ToTgString().ToString()
                    )
                ],
                TgButtonProvider.Applications.CancelRevokeApplication.ToArray()
            ]);

            var statusText = application.State switch
            {
                ApplicationStatus.Canceled => "Submitted for Review",
                _ => "Unknown Status"
            };

            await _tg.SendMessage(
                chatId: chatId,
                text: $"⚠️ **Confirm Application Revocation**\n\n" +
                      $"You are about to revoke your application for:\n\n" +
                      $"**📄 {application.Vacancy?.Title}**\n" +
                      $"Status: {statusText}\n" +
                      $"Submitted: {application.LastActivity:MMM dd, yyyy}\n\n" +
                      $"⚠️ **This action cannot be undone.**\n\n" +
                      $"Are you sure you want to revoke this application?",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct
            );

            await _tg.AnswerCallbackQuery(
                callbackQueryId: query.Id,
                text: "Confirm revocation",
                showAlert: false,
                cancellationToken: ct
            );

            _logger.LogInformation(
                "User {BotUserId} requested revocation confirmation for application {ApplicationId} in chat {ChatId}",
                botUserId, application.Id, chatId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling RevokeSpecificApplicationCallback for application {ApplicationId}", 
                command.ApplicationId);
            
            await _tg.SendMessage(
                chatId: chatId,
                text: "❌ An error occurred while processing your request. Please try again.",
                cancellationToken: ct
            );
        }
    }
}
