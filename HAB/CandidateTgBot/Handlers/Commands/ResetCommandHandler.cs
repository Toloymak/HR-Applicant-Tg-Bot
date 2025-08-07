using Telegram.Bot;

namespace CandidateTgBot.Handlers.Commands;

public class CandidateResetCommandHandler : IMenuBotCommand
{
    private readonly ITelegramBotClient _botClient;
    
    public static string Command => "reset";
    public static string Description => "Reset the bot";
    
    public CandidateResetCommandHandler(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }
    
    public async Task HandleCommand(long chatId, CancellationToken ct)
    {
        await _botClient.SendMessage(
            chatId: chatId,
            text: "Your bot state has been reset.",
            cancellationToken: ct
        );
    }

}