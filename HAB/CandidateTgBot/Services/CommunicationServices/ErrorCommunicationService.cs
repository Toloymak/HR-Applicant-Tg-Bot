using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Services.CommunicationServices;

public interface ISendUnableToIdentifyMessage
{
    Task Send(
        long chatId,
        CancellationToken ct);
}

public interface ISendCommandParsingErrorMessage
{
    Task Send(
        long chatId,
        CallbackQuery callback,
        CancellationToken ct);
}

public class ErrorCommunicationService : ISendUnableToIdentifyMessage, ISendCommandParsingErrorMessage
{
    private readonly ITelegramBotClient _tgClient;

    public ErrorCommunicationService(
        ITelegramBotClient tgClient)
    {
        _tgClient = tgClient;
    }

    async Task ISendUnableToIdentifyMessage.Send(
        long chatId,
        CancellationToken ct)
    {
        await _tgClient.SendMessage(
            chatId: chatId,
            text: "❌ **Unable to Identify User**\n\n" +
                  "Please try again or contact support if the issue persists.",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            cancellationToken: ct
        );
    }

    public async Task Send(
        long chatId,
        CallbackQuery callback,
        CancellationToken ct)
    {
        await _tgClient.SendMessage(
            chatId: chatId,
            text: $"Error handling command ({callback.Data})",
            cancellationToken: ct
        );
                
        await _tgClient.AnswerCallbackQuery(
            callbackQueryId: callback.Id,
            text: "Sorry, I couldn't parse the command, contact support",
            showAlert: true,
            cancellationToken: ct
        );
    }
}