using CandateTgBot.Shared.Services;
using CandidateTgBot.Types.Callbacks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public class VacancyInfoCallbackHandler : ICallbackHandler<VacancyInfoCallback>
{
    private readonly ITelegramBotClient _tg;
    private readonly IGetVacancyInfo _getVacancyInfo;

    public VacancyInfoCallbackHandler(ITelegramBotClient tg, IGetVacancyInfo getVacancyInfo)
    {
        _tg = tg;
        _getVacancyInfo = getVacancyInfo;
    }

    public async Task Handle(
        long chatId,
        VacancyInfoCallback command, 
        CallbackQuery callbackQuery,
        CancellationToken ct)
    {
        var info = await _getVacancyInfo.Get(command.VacancyId, ct);
        if (info is null)
        {
            await _tg.SendMessage(chatId, "Vacancy was not found.", cancellationToken: ct);
            return;
        }

        var text = $"{info.Title}\n\n{info.Description}";
        var keyboard = new InlineKeyboardMarkup([
            [
                InlineKeyboardButton.WithCallbackData(
                    text: "Apply",
                    callbackData: new ApplyForVacancyCallback { VacancyId = command.VacancyId }
                        .ToTgString().ToString()
                ),
                InlineKeyboardButton.WithCallbackData(
                    text: "Show others",
                    callbackData: new ShowOtherVacanciesCallback()
                        .ToTgString().ToString()
                )
            ]
        ]);

        await _tg.SendMessage(chatId, text, replyMarkup: keyboard, cancellationToken: ct);
    }
}
