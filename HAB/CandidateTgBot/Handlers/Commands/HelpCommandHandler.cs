using Telegram.Bot;

namespace CandidateTgBot.Handlers.Commands;

public class HelpCommandHandler : IMenuBotCommand
{
    private readonly ITelegramBotClient _botClient;

    public HelpCommandHandler(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task HandleCommand(long chatId, CancellationToken ct)
    {
        var helpText = "This bot will help ypu to apply to the position!";

        await _botClient.SendMessage(
            chatId: chatId,
            text: helpText,
            cancellationToken: ct
        );
    }

    public static string Command => "help";
    public static string Description => "Help";
}