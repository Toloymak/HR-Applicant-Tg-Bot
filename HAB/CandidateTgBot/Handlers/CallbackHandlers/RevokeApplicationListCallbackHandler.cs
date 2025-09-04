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

public class RevokeApplicationListCallbackHandler : ICallbackHandler<RevokeApplicationListCallback>
{
    private readonly ITelegramBotClient _tg;
    private readonly HrBotContext _context;
    private readonly ILogger<RevokeApplicationListCallbackHandler> _logger;
    private readonly ISendUnableToIdentifyMessage _sendUnableToIdentifyMessage;
    private readonly IProvideUserFromCallback _userProvider;

    public RevokeApplicationListCallbackHandler(
        ITelegramBotClient tg,
        BotUserService botUserService,
        HrBotContext context,
        ILogger<RevokeApplicationListCallbackHandler> logger,
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
        RevokeApplicationListCallback command,
        CallbackQuery query,
        CancellationToken ct)
    {
        
        var botUserId = await _userProvider.GetBotUserId(query, ct);
        if (botUserId is null)
        {
            await _sendUnableToIdentifyMessage.Send(chatId, ct);
            return;
        }

            // Get applications that can be revoked (CompletedByUser and CompetedByUserAndStartedNew status)
            var revokableApplications = await _context.UserApplications
                .Include(a => a.Vacancy)
                .Where(a => a.BotUserId == botUserId.Value && 
                           (a.State == ApplicationStatus.ReviewByHr))
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
                TgButtonProvider.Applications.RemoveApplication(
                    title: app.Vacancy?.Title ?? "Unknown Vacancy",
                    app.Id)
            }).ToList();

            // Add cancel button
            keyboardButtons.Add(TgButtonProvider.Applications.Status.ToArray());

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
}
