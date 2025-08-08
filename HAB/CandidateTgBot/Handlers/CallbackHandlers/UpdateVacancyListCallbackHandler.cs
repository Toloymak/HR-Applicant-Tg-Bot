using CandidateTgBot.Services.CommunicationServices;
using CandidateTgBot.Types.Callbacks;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public class UpdateVacancyListCallbackHandler
    : ICallbackHandler<UpdateVacancyListCallback>
{
    private readonly WelcomeMessageService _welcome;

    public UpdateVacancyListCallbackHandler(WelcomeMessageService welcome)
    {
        _welcome = welcome;
    }

    public async Task Handle(long chatId, UpdateVacancyListCallback command, CancellationToken ct)
    {
        // Mock behavior: resends the welcome + positions list as a refresh.
        await _welcome.SendWelcomeMessageAsync(chatId, ct);
    }
}

