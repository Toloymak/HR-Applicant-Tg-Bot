using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace CandidateTgBot.Handlers;

public class ErrorHandler
{
    private readonly ILogger<ErrorHandler> _logger;

    public ErrorHandler(ILogger<ErrorHandler> logger)
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