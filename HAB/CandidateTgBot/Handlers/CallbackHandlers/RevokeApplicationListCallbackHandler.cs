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

public class RevokeApplicationListCallbackHandler : ICallbackHandler<RevokeApplicationListCallback>
{
    private readonly ITelegramBotClient _tg;
    private readonly BotUserService _botUserService;
    private readonly HrBotContext _context;
    private readonly ILogger<RevokeApplicationListCallbackHandler> _logger;

    public RevokeApplicationListCallbackHandler(
        ITelegramBotClient tg,
        BotUserService botUserService,
        HrBotContext context,
        ILogger<RevokeApplicationListCallbackHandler> logger)
    {
        _tg = tg;
        _botUserService = botUserService;
        _context = context;
        _logger = logger;
    }

    public async Task Handle(
        long chatId,
        RevokeApplicationListCallback command,
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

            // Get applications that can be revoked (CompletedByUser and CompetedByUserAndStartedNew status)
            var revokableApplications = await _context.UserApplications
                .Include(a => a.Vacancy)
                .Where(a => a.BotUserId == botUserId.Value && 
                           (a.State == ApplicationStatus.CompletedByUser ||
                            a.State == ApplicationStatus.CompetedByUserAndStartedNew))
                .OrderByDescending(a => a.LastActivity)
                .ToListAsync(ct);

            if (!revokableApplications.Any())
            {
                await _tg.SendMessage(
                    chatId: chatId,
                    text: "❌ **No Applications to Revoke**\n\n" +
                          "You don't have any applications that can be revoked.\n\n" +
                          "💡 *Only applications with 'Submitted for Review' or 'Review by HR' status can be revoked.*",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    cancellationToken: ct
                );
                return;
            }

            // Create keyboard with revoke buttons
            var keyboardButtons = revokableApplications.Select(app => new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    text: $"🗑️ Revoke {app.Vacancy?.Title}",
                    callbackData: new RevokeSpecificApplicationCallback { ApplicationId = app.Id }
                        .ToTgString().ToString()
                )
            }).ToList();

            // Add cancel button
            keyboardButtons.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    text: "❌ Cancel",
                    callbackData: new CancelRevokeCallback().ToTgString().ToString()
                )
            });

            var keyboard = new InlineKeyboardMarkup(keyboardButtons);

            await _tg.SendMessage(
                chatId: chatId,
                text: "🗑️ **Revoke Application**\n\n" +
                      "Select the application you want to revoke:\n\n" +
                      "⚠️ *Warning: This action cannot be undone.*",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct
            );

            await _tg.AnswerCallbackQuery(
                callbackQueryId: query.Id,
                text: "Select application to revoke",
                showAlert: false,
                cancellationToken: ct
            );

            _logger.LogInformation(
                "User {BotUserId} requested revoke application list in chat {ChatId}",
                botUserId, chatId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling RevokeApplicationListCallback");
            
            await _tg.SendMessage(
                chatId: chatId,
                text: "❌ An error occurred while loading applications. Please try again.",
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
