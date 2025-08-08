using CandidateTgBot.Services.CommunicationServices;
using Telegram.Bot;

namespace CandidateTgBot.Handlers.Commands;

public class CandidateResetCommandHandler : IMenuBotCommand
{
    private readonly ITelegramBotClient _botClient;
    private readonly WelcomeMessageService _welcomeMessageService;
    
    public static string Command => "reset";
    public static string Description => "Reset the bot";
    
    public CandidateResetCommandHandler(ITelegramBotClient botClient, WelcomeMessageService welcomeMessageService)
    {
        _botClient = botClient;
        _welcomeMessageService = welcomeMessageService;
    }
    
    public async Task HandleCommand(long chatId, CancellationToken ct)
    {
        await _botClient.SendMessage(
            chatId: chatId,
            text: "Your bot state has been reset.",
            cancellationToken: ct
        );
        
        await _welcomeMessageService.SendWelcomeMessageAsync(chatId, ct);
    }

}