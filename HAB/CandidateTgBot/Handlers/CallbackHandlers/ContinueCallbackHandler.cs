using CandidateTgBot.Services;
using CandidateTgBot.Services.CommunicationServices;
using CandidateTgBot.Services.DataProviders;
using CandidateTgBot.Types.Callbacks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public class ContinueCallbackHandler : ICallbackHandler<ContinueCallback>
{
    private readonly BotMessageService _botMessageService;
    private readonly IProvideUserFromCallback _userProvider;
    private readonly ISendUnableToIdentifyMessage _sendUnableToIdentifyMessage;
    private readonly ITelegramBotClient _tg;

    public ContinueCallbackHandler(
        BotMessageService botMessageService,
        IProvideUserFromCallback userProvider,
        ISendUnableToIdentifyMessage sendUnableToIdentifyMessage,
        ITelegramBotClient tg)
    {
        _botMessageService = botMessageService;
        _userProvider = userProvider;
        _sendUnableToIdentifyMessage = sendUnableToIdentifyMessage;
        _tg = tg;
    }

    public async Task Handle(
        long chatId,
        ContinueCallback command,
        CallbackQuery callbackQuery,
        CancellationToken ct)
    {
        var botUserId = await _userProvider.GetBotUserId(callbackQuery, ct);
        if (botUserId is null)
        {
            await _sendUnableToIdentifyMessage.Send(chatId, ct);
            return;
        }
        
        await _botMessageService.SendContinueMessage(
            chatId, 
            botUserId, 
            ct);

        await _tg.AnswerCallbackQuery(
            callbackQueryId: callbackQuery.Id,
            cancellationToken: ct
        );
    }
}