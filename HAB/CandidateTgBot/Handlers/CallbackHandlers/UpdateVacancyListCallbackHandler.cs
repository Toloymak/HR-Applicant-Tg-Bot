using CandidateTgBot.Services.CommunicationServices;
using CandidateTgBot.Types.Callbacks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public class UpdateVacancyListCallbackHandler
    : ICallbackHandler<UpdateVacancyListCallback>
{
    private readonly VacancyListCommunicationService _vacancyListService;
    private readonly ITelegramBotClient _tgClient;

    public UpdateVacancyListCallbackHandler(
        VacancyListCommunicationService vacancyListService,
        ITelegramBotClient tgClient)
    {
        _vacancyListService = vacancyListService;
        _tgClient = tgClient;
    }

    public async Task Handle(
        long chatId,
        UpdateVacancyListCallback command,
        CallbackQuery callbackQuery,
        CancellationToken ct)
    {
        await _vacancyListService.SendVacancyListAsync(chatId, ct);

        if (callbackQuery.Message is { } msg)
        {
            await _tgClient.EditMessageReplyMarkup(
                chatId: msg.Chat.Id,
                messageId: msg.MessageId,
                replyMarkup: null,
                cancellationToken: ct
            );
        }
    }
}
