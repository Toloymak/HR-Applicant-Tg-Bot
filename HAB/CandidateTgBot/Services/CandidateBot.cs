using CandidateTgBot.Handlers;
using CandidateTgBot.Handlers.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Services;

public class CandidateBot
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<CandidateBot> _logger;
    private readonly IServiceProvider _serviceProvider;

    public CandidateBot(
        ITelegramBotClient botClient,
        ILogger<CandidateBot> logger,
        IServiceProvider serviceProvider)
    {
        _botClient = botClient;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task Start(CancellationToken ct)
    {
        await _botClient.SetMyCommands(
            MenuBotConfig.GetMenuCommands(),
            cancellationToken: ct);
        var me = await _botClient.GetMe(cancellationToken: ct);

        _logger.LogInformation(
            "Candidate Bot started with name: {BotName} and username: {BotUsername}",
            me.FirstName,
            me.Username
        );

        _botClient.StartReceiving(async (client, update, token) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<CandidateBotMessageHandler>();
                await handler.HandleUpdateAsync(client, update, token);
            },
            async (client, exception, token) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<CandidateBotErrorHandler>();
                await handler.HandleErrorAsync(client, exception, token);
            },
            cancellationToken: ct
        );
    }
}