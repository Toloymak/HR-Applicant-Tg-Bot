using CandidateTgBot.Services;
using CandidateTgBot.Types.Callbacks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public class ShowStatusCallbackHandler : ICallbackHandler<ShowStatusCallback>
{
    private readonly ITelegramBotClient _tg;
    private readonly BotUserService _botUserService;
    private readonly ApplicationStatusService _statusService;
    private readonly ILogger<ShowStatusCallbackHandler> _logger;

    public ShowStatusCallbackHandler(
        ITelegramBotClient tg,
        BotUserService botUserService,
        ApplicationStatusService statusService,
        ILogger<ShowStatusCallbackHandler> logger)
    {
        _tg = tg;
        _botUserService = botUserService;
        _statusService = statusService;
        _logger = logger;
    }

    public async Task Handle(
        long chatId,
        ShowStatusCallback command,
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

            // Show application status
            await _statusService.ShowApplicationStatusAsync(chatId, botUserId.Value, ct);

            await _tg.AnswerCallbackQuery(
                callbackQueryId: query.Id,
                text: "Showing your application status!",
                showAlert: false,
                cancellationToken: ct
            );

            _logger.LogInformation(
                "User {BotUserId} requested application status via button in chat {ChatId}",
                botUserId, chatId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling ShowStatusCallback");
            
            await _tg.SendMessage(
                chatId: chatId,
                text: "❌ An error occurred while loading your status. Please try again.",
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
