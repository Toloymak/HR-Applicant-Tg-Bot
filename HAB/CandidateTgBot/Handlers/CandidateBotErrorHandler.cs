using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace CandidateTgBot.Handlers;

public class CandidateBotErrorHandler
{
    private readonly ILogger<CandidateBotErrorHandler> _logger;

    public CandidateBotErrorHandler(ILogger<CandidateBotErrorHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleErrorAsync(
        ITelegramBotClient bot,
        Exception ex,
        CancellationToken token)
    {
        _logger.LogError(ex, "Error occurred: {Error}", ex.Message);
        return Task.CompletedTask;
    }
}