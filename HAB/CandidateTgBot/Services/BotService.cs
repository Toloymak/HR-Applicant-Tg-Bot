using CandidateTgBot.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace CandidateTgBot.Services;

public class CandidateBotService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<CandidateBotService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public CandidateBotService(
        ITelegramBotClient botClient,
        ILogger<CandidateBotService> logger,
        IServiceProvider serviceProvider)
    {
        _botClient = botClient;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task Start(CancellationToken ct)
    {
        var me = await _botClient.GetMe(cancellationToken: ct);

        _logger.LogInformation(
            "Candidate Bot started with name: {BotName} and username: {BotUsername}",
            me.FirstName,
            me.Username
        );

        _botClient.StartReceiving(async (client, update, token) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<MessageHandler>();
                await handler.HandleUpdateAsync(client, update, token);
            },
            async (client, exception, token) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<ErrorHandler>();
                await handler.HandleErrorAsync(client, exception, token);
            },
            cancellationToken: ct
        );
    }
}