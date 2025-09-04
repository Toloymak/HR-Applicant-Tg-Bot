using CandidateTgBot.Services.CommunicationServices;
using Telegram.Bot;

namespace CandidateTgBot.Handlers.Commands;

public class CandidateResetCommandHandler : IMenuBotCommand
{
    private readonly ITelegramBotClient _botClient;
    private readonly WelcomeCommunicationService _welcomeCommunicationService;
    
    public static string Command => "reset";
    public static string Description => "Reset the bot";
    
    public CandidateResetCommandHandler(
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
            text: "Your bot state has been reset.",
            cancellationToken: ct
        );
        
        await _welcomeCommunicationService.SendWelcomeMessageAsync(chatId, ct);
    }

}