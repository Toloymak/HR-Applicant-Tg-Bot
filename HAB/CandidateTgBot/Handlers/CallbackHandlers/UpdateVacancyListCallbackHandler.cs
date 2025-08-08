using CandidateTgBot.Services.CommunicationServices;
using CandidateTgBot.Types.Callbacks;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public class UpdateVacancyListCallbackHandler : ICallbackHandler<UpdateVacancyListCallback>
{
    private readonly VacancyListCommunicationService _vacancyListService;

    public UpdateVacancyListCallbackHandler(VacancyListCommunicationService vacancyListService)
    {
        _vacancyListService = vacancyListService;
    }

    public async Task Handle(long chatId, UpdateVacancyListCallback command, CancellationToken ct)
    {
        await _vacancyListService.SendVacancyListAsync(chatId, ct);
    }
}
