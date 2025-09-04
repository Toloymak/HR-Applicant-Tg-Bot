using CandidateTgBot.Services;
using CandidateTgBot.Services.CommunicationServices;
using CandidateTgBot.Services.DataProviders;
using CandidateTgBot.Types.Callbacks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public class CancelRevokeApplicationCallbackHandler : ICallbackHandler<CancelRevokeApplicationCallback>
{
    private readonly ITelegramBotClient _tg;
    private readonly ILogger<CancelRevokeApplicationCallbackHandler> _logger;
    private readonly StatusCommunicationService _statusCommunicationService;
    private readonly IProvideUserFromCallback _userProvider;
    private readonly ISendUnableToIdentifyMessage _sendUnableToIdentifyMessage;

    public CancelRevokeApplicationCallbackHandler(
        ITelegramBotClient tg,
        ILogger<CancelRevokeApplicationCallbackHandler> logger,
        StatusCommunicationService statusCommunicationService,
        IProvideUserFromCallback userProvider,
        ISendUnableToIdentifyMessage sendUnableToIdentifyMessage)
    {
        _tg = tg;
        _logger = logger;
        _statusCommunicationService = statusCommunicationService;
        _userProvider = userProvider;
        _sendUnableToIdentifyMessage = sendUnableToIdentifyMessage;
    }

    public async Task Handle(
        long chatId,
        CancelRevokeApplicationCallback command,
        CallbackQuery query,
        CancellationToken ct)
    {
        var botUser = await _userProvider.GetBotUser(query, ct);
        if (botUser is null)
        {
            await _sendUnableToIdentifyMessage.Send(chatId, ct);
            return;
        }

        await _statusCommunicationService.SendStatusInfo(chatId, botUser.Id, ct);
        
        // Answer the callback query to remove the loading state
        await _tg.AnswerCallbackQuery(
            callbackQueryId: query.Id,
            cancellationToken: ct
        );

        _logger.LogInformation(
            "User {UserId} chose to keep their application in chat {ChatId}",
            query.From?.Id, chatId);
    }
}