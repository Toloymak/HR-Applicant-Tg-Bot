using CandidateTgBot.Services.CommunicationServices;
using CandidateTgBot.Types.Callbacks;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public class ShowOtherVacanciesCallbackHandler
    : ICallbackHandler<ShowOtherVacanciesCallback>
{
    private readonly VacancyListCommunicationService _vacancyListService;

    public ShowOtherVacanciesCallbackHandler(
        VacancyListCommunicationService vacancyListService)
    {
        _vacancyListService = vacancyListService;
    }

    public async Task Handle(
        long chatId,
        ShowOtherVacanciesCallback command,
        CallbackQuery query,
        CancellationToken ct)
    {
        // Do not disable previous keyboard; just present the list again.
        await _vacancyListService.SendVacancyListAsync(chatId, ct);
    }
}

