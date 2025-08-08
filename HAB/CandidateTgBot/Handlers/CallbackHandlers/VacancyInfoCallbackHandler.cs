using CandidateTgBot.Types.Callbacks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public class VacancyInfoCallbackHandler : ICallbackHandler<VacancyInfoCallback>
{
    private readonly ITelegramBotClient _tg;

    public VacancyInfoCallbackHandler(ITelegramBotClient tg)
    {
        _tg = tg;
    }

    public async Task Handle(
        long chatId,
        VacancyInfoCallback command, 
        CallbackQuery callbackQuery,
        CancellationToken ct)
    {
        // Mock behavior: just acknowledge and echo the vacancy ID.
        await _tg.SendMessage(
            chatId: chatId,
            text: $"You selected vacancy: {command.VacancyId}",
            cancellationToken: ct
        );

        // Next steps could be: load vacancy details and send a form.
    }
}

