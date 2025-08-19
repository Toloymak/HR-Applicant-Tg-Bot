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

public class ConfirmResetCallbackHandler : ICallbackHandler<ConfirmResetCallback>
{
    private readonly ITelegramBotClient _tg;
    private readonly HrBotContext _context;
    private readonly ApplicationService _applicationService;
    private readonly BotUserService _botUserService;
    private readonly BotMessageService _botMessageService;
    private readonly ILogger<ConfirmResetCallbackHandler> _logger;

    public ConfirmResetCallbackHandler(
        ITelegramBotClient tg,
        HrBotContext context,
        ApplicationService applicationService,
        BotUserService botUserService,
        BotMessageService botMessageService,
        ILogger<ConfirmResetCallbackHandler> logger)
    {
        _tg = tg;
        _context = context;
        _applicationService = applicationService;
        _botUserService = botUserService;
        _botMessageService = botMessageService;
        _logger = logger;
    }

    public async Task Handle(
        long chatId,
        ConfirmResetCallback command,
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

            // Get all active applications for the user
            var activeApplications = await _applicationService.GetUserActiveApplicationsAsync(botUser.Id, ct);

            // Cancel all active applications
            foreach (var application in activeApplications)
            {
                await _applicationService.UpdateApplicationStatusAsync(
                    application.Id,
                    ApplicationStatus.CanceledByUser,
                    ct
                );
            }

            if (activeApplications.Count > 0)
            {
                await _tg.SendMessage(
                    chatId: chatId,
                    text: $"🔄 **Reset Complete**\n\n" +
                          $"Canceled {activeApplications.Count} active application{(activeApplications.Count > 1 ? "s" : "")}.\n\n" +
                          $"Starting fresh...",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    cancellationToken: ct
                );

                _logger.LogInformation(
                    "User {TgId} ({Username}) reset and canceled {ApplicationCount} applications",
                    botUser.TgId, botUser.TgName, activeApplications.Count);
            }
            else
            {
                await _tg.SendMessage(
                    chatId: chatId,
                    text: "🔄 **Reset Complete**\n\nStarting fresh...",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    cancellationToken: ct
                );
            }

            // Send fresh start message
            await _botMessageService.SendStartMessageAsync(chatId, botUser.Id, ct);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error confirming reset for user {ChatId}",
                chatId);

            await _tg.SendMessage(
                chatId: chatId,
                text: "❌ An error occurred while resetting. Please try again later.",
                cancellationToken: ct
            );
        }
    }
}
