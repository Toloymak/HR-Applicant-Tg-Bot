using CandidateTgBot.Services;

namespace Application.HostedSevices;

public class CandidateBotHostedService : IHostedService
{
    private readonly ILogger<CandidateBotHostedService> _logger;
    private readonly CandidateBot _bot;

    public CandidateBotHostedService(
        ILogger<CandidateBotHostedService> logger,
        CandidateBot bot)
    {
        _logger = logger;
        _bot = bot;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting Candidate Bot Hosted Service...");
        await _bot.Start(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Candidate Bot Hosted Service...");
        return Task.CompletedTask;
    }
}