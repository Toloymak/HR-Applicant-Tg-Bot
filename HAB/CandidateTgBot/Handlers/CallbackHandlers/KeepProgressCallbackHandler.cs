using CandidateTgBot.Types.Callbacks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public class KeepProgressCallbackHandler : ICallbackHandler<KeepProgressCallback>
{
    private readonly ITelegramBotClient _tg;
    private readonly ILogger<KeepProgressCallbackHandler> _logger;

    public KeepProgressCallbackHandler(
        ITelegramBotClient tg,
        ILogger<KeepProgressCallbackHandler> logger)
    {
        _tg = tg;
        _logger = logger;
    }

    public async Task Handle(
        long chatId,
        KeepProgressCallback command,
        CallbackQuery query,
        CancellationToken ct)
    {
        try
        {
            // Send positive reinforcement message
            await _tg.SendMessage(
                chatId: chatId,
                text: "👍 **Perfect! Your progress is safe.**\n\n" +
                      "You can continue working on your current application(s).\n\n" +
                      "💡 *Use /start to see your current status or continue where you left off.*",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                cancellationToken: ct
            );

            // Answer the callback query to remove the loading state
            await _tg.AnswerCallbackQuery(
                callbackQueryId: query.Id,
                text: "Progress kept successfully!",
                cancellationToken: ct
            );

            _logger.LogInformation(
                "User {UserId} chose to keep their progress in chat {ChatId}",
                query.From?.Id, chatId);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error handling keep progress callback for user {ChatId}",
                chatId);

            await _tg.SendMessage(
                chatId: chatId,
                text: "✅ Your progress is safe!",
                cancellationToken: ct
            );
        }
    }
}
