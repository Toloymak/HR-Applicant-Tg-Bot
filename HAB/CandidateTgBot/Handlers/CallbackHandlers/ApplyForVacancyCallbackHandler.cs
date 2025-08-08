using CandidateTgBot.Types.Callbacks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public class ApplyForVacancyCallbackHandler : ICallbackHandler<ApplyForVacancyCallback>
{
    private readonly ITelegramBotClient _tg;

    public ApplyForVacancyCallbackHandler(ITelegramBotClient tg)
    {
        _tg = tg;
    }

    public async Task Handle(
        long chatId,
        ApplyForVacancyCallback command,
        CallbackQuery query,
        CancellationToken ct)
    {
        // Mock: acknowledge application start
        await _tg.SendMessage(
            chatId: chatId,
            text: $"Starting application for vacancy {command.VacancyId}. (mock)",
            cancellationToken: ct
        );
    }
}

