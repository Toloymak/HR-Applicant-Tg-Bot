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
        var helpText = 
            "🏢 *Welcome to the HR Candidate Bot!*\n\n" +
            "This bot helps you apply for job positions. Here are the available commands:\n\n" +
            "• /start - Start using the bot and begin your application\n" +
            "• /help - Show this help message\n" +
            "• /reset - Reset your session and start over\n\n" +
            "💡 *How to use:*\n" +
            "Send /start to begin the application process and browse available vacancies!";

        await _botClient.SendMessage(
            chatId: chatId,
            text: helpText,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            cancellationToken: ct
        );
    }

    public static string Command => "help";
    public static string Description => "Help";
}