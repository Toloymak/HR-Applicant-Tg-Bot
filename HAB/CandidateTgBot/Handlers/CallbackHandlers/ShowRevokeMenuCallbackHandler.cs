using CandidateTgBot.Services;
using CandidateTgBot.Services.CommunicationServices;
using CandidateTgBot.Services.DataProviders;
using CandidateTgBot.Types.Callbacks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public class ShowRevokeMenuCallbackHandler : ICallbackHandler<ShowRevokeListCallback>
{
    private readonly ITelegramBotClient _tg;
    private readonly StatusCommunicationService _statusCommunicationService;
    private readonly ILogger<ShowRevokeMenuCallbackHandler> _logger;
    private readonly ISendUnableToIdentifyMessage _sendUnableToIdentifyMessage;
    private readonly IProvideUserFromCallback _userProvider;

    public ShowRevokeMenuCallbackHandler(
        ITelegramBotClient tg,
        BotUserService botUserService,
        ILogger<ShowRevokeMenuCallbackHandler> logger,
        ISendUnableToIdentifyMessage unableToIdentifyMessageNotifier,
        IProvideUserFromCallback userProvider,
        StatusCommunicationService statusCommunicationService)
    {
        _tg = tg;
        _logger = logger;
        _sendUnableToIdentifyMessage = unableToIdentifyMessageNotifier;
        _userProvider = userProvider;
        _statusCommunicationService = statusCommunicationService;
    }

    public async Task Handle(
        long chatId,
        ShowRevokeListCallback command,
        CallbackQuery query,
        CancellationToken ct)
    {
        var botUserId = await _userProvider.GetBotUserId(query, ct);
        if (botUserId is null)
        {
            await _sendUnableToIdentifyMessage.Send(chatId, ct);
            return;
        }

        // Send cancellation message
        await _tg.SendMessage(
            chatId: chatId,
            text: "✅ **Revocation Cancelled**\n\n" +
                  "Your application revocation has been cancelled.\n\n" +
                  "💡 *Your applications remain unchanged.*",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            cancellationToken: ct
        );

        // Show status again
        await _statusCommunicationService.SendStatusInfo(chatId, botUserId.Value, ct);

        await _tg.AnswerCallbackQuery(
            callbackQueryId: query.Id,
            text: "Revocation cancelled",
            showAlert: false,
            cancellationToken: ct
        );

        _logger.LogInformation(
            "User {BotUserId} cancelled application revocation in chat {ChatId}",
            botUserId, chatId);
    }
}