using CandidateTgBot.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace CandidateTgBot.Handlers.Commands;

public class ContinueCommandHandler : IMenuBotCommand
{
    private readonly BotMessageService _botMessageService;

    public static string Command => "continue";
    public static string Description => "Continue your current application";
    
    public ContinueCommandHandler(
        ITelegramBotClient botClient, 
        BotMessageService botMessageService,
        ILogger<ContinueCommandHandler> logger)
    {
        _botMessageService = botMessageService;
    }
    
    public async Task HandleCommand(long chatId, CancellationToken ct)
    {
        await _botMessageService.SendContinueMessage(chatId, null, ct);
    }
}
