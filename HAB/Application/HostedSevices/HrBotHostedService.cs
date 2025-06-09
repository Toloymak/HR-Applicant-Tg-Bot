namespace Application.HostedSevices;

public class HrBotHostedService : IHostedService
{
    private readonly ILogger<HrBotHostedService> _logger;

    public HrBotHostedService(ILogger<HrBotHostedService> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting Candidate Bot Hosted Service...");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Candidate Bot Hosted Service...");
        return Task.CompletedTask;
    }
}