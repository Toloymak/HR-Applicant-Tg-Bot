using CandidateTgBot.Types.Callbacks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public class KeepApplicationCallbackHandler : ICallbackHandler<KeepApplicationCallback>
{
    private readonly ITelegramBotClient _tg;
    private readonly ILogger<KeepApplicationCallbackHandler> _logger;

    public KeepApplicationCallbackHandler(
        ITelegramBotClient tg,
        ILogger<KeepApplicationCallbackHandler> logger)
    {
        _tg = tg;
        _logger = logger;
    }

    public async Task Handle(
        long chatId,
        KeepApplicationCallback command,
        CallbackQuery query,
        CancellationToken ct)
    {
        try
        {
            // Send positive reinforcement message
            await _tg.SendMessage(
                chatId: chatId,
                text: "🎯 **Great, let's continue!**\n\n" +
                      "Your application is safe and you can continue working on it.\n\n" +
                      "💡 *Use /continue to resume where you left off.*",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                cancellationToken: ct
            );

            // Answer the callback query to remove the loading state
            await _tg.AnswerCallbackQuery(
                callbackQueryId: query.Id,
                text: "Application kept successfully!",
                cancellationToken: ct
            );

            _logger.LogInformation(
                "User {UserId} chose to keep their application in chat {ChatId}",
                query.From?.Id, chatId);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error handling keep application callback for user {ChatId}",
                chatId);

            await _tg.SendMessage(
                chatId: chatId,
                text: "✅ Your application is safe!",
                cancellationToken: ct
            );
        }
    }
}
