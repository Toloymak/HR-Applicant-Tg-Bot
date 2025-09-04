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

namespace CandidateTgBot.Handlers.CallbackHandlers;

public class StartNewApplicationCallbackHandler : ICallbackHandler<StartNewApplicationCallback>
{
    private readonly ITelegramBotClient _tg;
    private readonly WelcomeCommunicationService _welcomeService;
    private readonly HrBotContext _context;
    private readonly ILogger<StartNewApplicationCallbackHandler> _logger;
    private readonly IProvideUserFromCallback _userProvider;
    private readonly ISendUnableToIdentifyMessage _sendUnableToIdentifyMessage;

    public StartNewApplicationCallbackHandler(
        ITelegramBotClient tg,
        BotUserService botUserService,
        WelcomeCommunicationService welcomeService,
        HrBotContext context,
        ILogger<StartNewApplicationCallbackHandler> logger,
        IProvideUserFromCallback userProvider,
        ISendUnableToIdentifyMessage sendUnableToIdentifyMessage)
    {
        _tg = tg;
        _welcomeService = welcomeService;
        _context = context;
        _logger = logger;
        _userProvider = userProvider;
        _sendUnableToIdentifyMessage = sendUnableToIdentifyMessage;
    }

    public async Task Handle(
        long chatId,
        StartNewApplicationCallback command,
        CallbackQuery query,
        CancellationToken ct)
    {
        var botUserId = await _userProvider.GetBotUserId(query, ct);
        if (botUserId is null)
        {
            await _sendUnableToIdentifyMessage.Send(chatId, ct);
            return;
        }

            // Check if user has active applications and update their status
            var activeApplications = await _context.UserApplications
                .Where(a => a.BotUserId == botUserId.Value && 
                           a.State == ApplicationStatus.InProgress)
                .ToListAsync(ct);

            if (activeApplications.Any())
            {
                // Update all active applications to CompetedByUserAndStartedNew
                foreach (var application in activeApplications)
                {
                    application.State = ApplicationStatus.Canceled;
                    application.LastActivity = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync(ct);

                _logger.LogInformation(
                    "Updated {Count} active applications to CompetedByUserAndStartedNew for user {BotUserId}",
                    activeApplications.Count, botUserId.Value);
            }

            // Send start message
            await _tg.SendMessage(
                chatId: chatId,
                text: "🚀 Welcome! Let's get started with your job application.",
                cancellationToken: ct
            );

            // Show welcome message and vacancy list
            await _welcomeService.SendWelcomeMessageAsync(chatId, botUserId, ct);

            await _tg.AnswerCallbackQuery(
                callbackQueryId: query.Id,
                text: "Starting new application!",
                showAlert: false,
                cancellationToken: ct
            );

            _logger.LogInformation(
                "User {BotUserId} started new application via button in chat {ChatId}",
                botUserId, chatId);

    }
}
