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

public class SendOneMoreApplicationCallbackHandler : ICallbackHandler<SendOneMoreApplicationCallback>
{
    private readonly ITelegramBotClient _tg;
    private readonly BotUserService _botUserService;
    private readonly ApplicationService _applicationService;
    private readonly HrBotContext _context;
    private readonly WelcomeCommunicationService _welcomeService;
    private readonly ILogger<SendOneMoreApplicationCallbackHandler> _logger;

    public SendOneMoreApplicationCallbackHandler(
        ITelegramBotClient tg,
        BotUserService botUserService,
        ApplicationService applicationService,
        HrBotContext context,
        WelcomeCommunicationService welcomeService,
        ILogger<SendOneMoreApplicationCallbackHandler> logger)
    {
        _tg = tg;
        _botUserService = botUserService;
        _applicationService = applicationService;
        _context = context;
        _welcomeService = welcomeService;
        _logger = logger;
    }

    public async Task Handle(
        long chatId,
        SendOneMoreApplicationCallback command,
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

            // Update the completed application status
            await _applicationService.UpdateApplicationStatusAsync(
                command.CompletedApplicationId, 
                ApplicationStatus.CompetedByUserAndStartedNew, 
                ct);

            // Send confirmation message
            await _tg.SendMessage(
                chatId: chatId,
                text: "🎯 **Ready for Another Application!**\n\n" +
                      "Your previous application has been marked as completed.\n\n" +
                      "Here are the available positions:",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                cancellationToken: ct
            );

            // Show available vacancies
            await _welcomeService.SendWelcomeMessageAsync(chatId, botUserId, ct);

            await _tg.AnswerCallbackQuery(
                callbackQueryId: query.Id,
                text: "Showing available positions!",
                showAlert: false,
                cancellationToken: ct
            );

            _logger.LogInformation(
                "User {BotUserId} marked application {ApplicationId} as completed and started new",
                botUserId, command.CompletedApplicationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling SendOneMoreApplicationCallback for application {ApplicationId}", 
                command.CompletedApplicationId);
            
            await _tg.SendMessage(
                chatId: chatId,
                text: "❌ An error occurred while processing your request. Please try again.",
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
