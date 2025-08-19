using CandidateTgBot.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace CandidateTgBot.Handlers.Commands;

public class ContinueCommandHandler : IMenuBotCommand
{
    private readonly ITelegramBotClient _botClient;
    private readonly BotMessageService _botMessageService;
    private readonly ILogger<ContinueCommandHandler> _logger;
    
    public static string Command => "continue";
    public static string Description => "Continue your current application";
    
    public ContinueCommandHandler(
        ITelegramBotClient botClient, 
        BotMessageService botMessageService,
        ILogger<ContinueCommandHandler> logger)
    {
        _botClient = botClient;
        _botMessageService = botMessageService;
        _logger = logger;
    }
    
    public async Task HandleCommand(long chatId, CancellationToken ct)
    {
        await _botMessageService.SendContinueMessageAsync(chatId, null, ct);
    }
}
