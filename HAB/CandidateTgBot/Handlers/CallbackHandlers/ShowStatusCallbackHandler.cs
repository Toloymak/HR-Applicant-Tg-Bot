using CandidateTgBot.Services;
using CandidateTgBot.Services.CommunicationServices;
using CandidateTgBot.Services.DataProviders;
using CandidateTgBot.Types.Callbacks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public class ShowStatusCallbackHandler : ICallbackHandler<ShowStatusCallback>
{
    private readonly ITelegramBotClient _tg;
    private readonly ILogger<ShowStatusCallbackHandler> _logger;
    private readonly IProvideUserFromCallback _userProvider;
    private readonly ISendUnableToIdentifyMessage _sendUnableToIdentifyMessage;
    private readonly StatusCommunicationService _statusCommunicationService;

    public ShowStatusCallbackHandler(
        ITelegramBotClient tg,
        BotUserService botUserService,
        ILogger<ShowStatusCallbackHandler> logger,
        IProvideUserFromCallback userProvider,
        ISendUnableToIdentifyMessage sendUnableToIdentifyMessage,
        StatusCommunicationService statusCommunicationService)
    {
        _tg = tg;
        _logger = logger;
        _userProvider = userProvider;
        _sendUnableToIdentifyMessage = sendUnableToIdentifyMessage;
        _statusCommunicationService = statusCommunicationService;
    }

    public async Task Handle(
        long chatId,
        ShowStatusCallback command,
        CallbackQuery query,
        CancellationToken ct)
    {
        var botUserId = await _userProvider.GetBotUserId(query, ct);
        if (botUserId is null)
        {
            await _sendUnableToIdentifyMessage.Send(chatId, ct);
            return;
        }

        await _statusCommunicationService.SendStatusInfo(chatId, botUserId.Value, ct);

            await _tg.AnswerCallbackQuery(
                callbackQueryId: query.Id,
                text: "Showing your application status!",
                showAlert: false,
                cancellationToken: ct
            );

            _logger.LogInformation(
                "User {BotUserId} requested application status via button in chat {ChatId}",
                botUserId, chatId);
    }
}
