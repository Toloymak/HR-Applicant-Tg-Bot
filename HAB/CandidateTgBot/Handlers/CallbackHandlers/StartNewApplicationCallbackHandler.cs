using CandidateTgBot.Services;
using CandidateTgBot.Services.CommunicationServices;
using CandidateTgBot.Types.Callbacks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public class StartNewApplicationCallbackHandler : ICallbackHandler<StartNewApplicationCallback>
{
    private readonly ITelegramBotClient _tg;
    private readonly BotUserService _botUserService;
    private readonly WelcomeCommunicationService _welcomeService;
    private readonly ILogger<StartNewApplicationCallbackHandler> _logger;

    public StartNewApplicationCallbackHandler(
        ITelegramBotClient tg,
        BotUserService botUserService,
        WelcomeCommunicationService welcomeService,
        ILogger<StartNewApplicationCallbackHandler> logger)
    {
        _tg = tg;
        _botUserService = botUserService;
        _welcomeService = welcomeService;
        _logger = logger;
    }

    public async Task Handle(
        long chatId,
        StartNewApplicationCallback command,
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling StartNewApplicationCallback");
            
            await _tg.SendMessage(
                chatId: chatId,
                text: "❌ An error occurred while starting your application. Please try again.",
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
