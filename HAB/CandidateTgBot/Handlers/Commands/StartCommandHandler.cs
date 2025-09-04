using CandidateTgBot.Services.CommunicationServices;
using Telegram.Bot;

namespace CandidateTgBot.Handlers.Commands;

public class StartCommandHandler : IMenuBotCommand
{
    private readonly ITelegramBotClient _botClient;
    private readonly WelcomeCommunicationService _welcomeCommunicationService;
    
    public static string Command => "start";
    public static string Description => "Start using the bot";
    
    public StartCommandHandler(
        ITelegramBotClient botClient,
        WelcomeCommunicationService welcomeCommunicationService)
    {
        _botClient = botClient;
        _welcomeCommunicationService = welcomeCommunicationService;
    }
    
    public async Task HandleCommand(long chatId, CancellationToken ct)
    {
        await _botClient.SendMessage(
            chatId: chatId,
            text: "🚀 Welcome! Let's get started with your job application.",
            cancellationToken: ct
        );
        
        await _welcomeCommunicationService.SendWelcomeMessageAsync(chatId, ct);
    }
}
