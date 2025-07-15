using CandidateTgBot.Services;

namespace Application.HostedSevices;

public class CandidateBotHostedService : IHostedService
{
    private readonly ILogger<CandidateBotHostedService> _logger;
    private readonly CandidateBotService _botService;

    public CandidateBotHostedService(
        ILogger<CandidateBotHostedService> logger,
        CandidateBotService botService)
    {
        _logger = logger;
        _botService = botService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting Candidate Bot Hosted Service...");
        await _botService.Start(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Candidate Bot Hosted Service...");
        return Task.CompletedTask;
    }
}