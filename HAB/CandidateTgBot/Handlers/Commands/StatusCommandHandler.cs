using CandidateTgBot.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace CandidateTgBot.Handlers.Commands;

public class StatusCommandHandler : IMenuBotCommand
{
    private readonly ITelegramBotClient _botClient;
    private readonly BotMessageService _botMessageService;
    private readonly ILogger<StatusCommandHandler> _logger;
    
    public static string Command => "status";
    public static string Description => "Show your application status";
    
    public StatusCommandHandler(
        ITelegramBotClient botClient, 
        BotMessageService botMessageService,
        ILogger<StatusCommandHandler> logger)
    {
        _botClient = botClient;
        _botMessageService = botMessageService;
        _logger = logger;
    }
    
    public async Task HandleCommand(long chatId, CancellationToken ct)
    {
        await _botMessageService.SendStatusMessageAsync(chatId, null, ct);
    }
}
