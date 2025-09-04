using CandidateTgBot.Services;
using CandidateTgBot.Services.CommunicationServices;
using CandidateTgBot.Services.DataProviders;
using CandidateTgBot.Types.Callbacks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public class AnswerNoCallbackHandler : ICallbackHandler<AnswerNoCallback>
{
    private readonly ITelegramBotClient _tg;
    private readonly AnswerService _answerService;
    private readonly ILogger<AnswerNoCallbackHandler> _logger;
    private readonly IProvideUserFromCallback _userProvider;
    private readonly ISendUnableToIdentifyMessage _sendUnableToIdentifyMessage;
    private readonly AnswerResultCommunicationService _answerResultCS;

    public AnswerNoCallbackHandler(
        ITelegramBotClient tg,
        AnswerService answerService,
        BotUserService botUserService,
        ILogger<AnswerNoCallbackHandler> logger,
        IProvideUserFromCallback userProvider,
        ISendUnableToIdentifyMessage sendUnableToIdentifyMessage,
        AnswerResultCommunicationService answerResultCS)
    {
        _tg = tg;
        _answerService = answerService;
        _logger = logger;
        _userProvider = userProvider;
        _sendUnableToIdentifyMessage = sendUnableToIdentifyMessage;
        _answerResultCS = answerResultCS;
    }

    public async Task Handle(
        long chatId,
        AnswerNoCallback command,
        CallbackQuery query,
        CancellationToken ct)
    {
        var botUserId = await _userProvider.GetBotUserId(query, ct);
        if (botUserId is null)
        {
            await _sendUnableToIdentifyMessage.Send(chatId, ct);
            return;
        }

        var success = await _answerService.SaveBooleanAnswerAsync(
            chatId, botUserId.Value, command.QuestionId, false, ct);

        await _answerResultCS.SendAnswerStatus(query, success, ct);

        _logger.LogInformation(
            "User answered 'No' to question {QuestionId} in chat {ChatId}",
            command.QuestionId, chatId);
    }

}
