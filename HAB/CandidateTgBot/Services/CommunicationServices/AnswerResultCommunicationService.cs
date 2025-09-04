using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Services.CommunicationServices;

public class AnswerResultCommunicationService
{
    private readonly ITelegramBotClient _tg;

    public AnswerResultCommunicationService(ITelegramBotClient tg)
    {
        _tg = tg;
    }

    public async Task SendAnswerStatus(
        CallbackQuery query,
        bool success,
        CancellationToken ct)
    {
        if (success)
        {
            await _tg.AnswerCallbackQuery(
                callbackQueryId: query.Id,
                text: "Answer saved!",
                showAlert: false,
                cancellationToken: ct
            );
        }
        else
        {
            await _tg.AnswerCallbackQuery(
                callbackQueryId: query.Id,
                text: "Error saving answer",
                showAlert: true,
                cancellationToken: ct
            );
        }
    }
}