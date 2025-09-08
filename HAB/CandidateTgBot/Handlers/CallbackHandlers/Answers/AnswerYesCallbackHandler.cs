using CandidateTgBot.Services;
using CandidateTgBot.Services.CommunicationServices;
using CandidateTgBot.Services.DataProviders;
using CandidateTgBot.Types.Callbacks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers.CallbackHandlers.Answers;

public class AnswerYesCallbackHandler : ICallbackHandler<AnswerYesCallback>
{
    private readonly ITelegramBotClient _tg;
    private readonly AnswerService _answerService;
    private readonly BotUserService _botUserService;
    private readonly ILogger<AnswerYesCallbackHandler> _logger;
    private readonly IProvideUserFromCallback _userProvider;
    private readonly ISendUnableToIdentifyMessage _sendUnableToIdentifyMessage;
    private readonly AnswerResultCommunicationService _answerResultCS;

    public AnswerYesCallbackHandler(
        ITelegramBotClient tg,
        AnswerService answerService,
        BotUserService botUserService,
        ILogger<AnswerYesCallbackHandler> logger,
        IProvideUserFromCallback userProvider,
        ISendUnableToIdentifyMessage sendUnableToIdentifyMessage,
        AnswerResultCommunicationService answerResultCS)
    {
        _tg = tg;
        _answerService = answerService;
        _botUserService = botUserService;
        _logger = logger;
        _userProvider = userProvider;
        _sendUnableToIdentifyMessage = sendUnableToIdentifyMessage;
        _answerResultCS = answerResultCS;
    }

    public async Task Handle(
        long chatId,
        AnswerYesCallback command,
        CallbackQuery query,
        CancellationToken ct)
    {
        var botUserId = await _userProvider.GetBotUserId(query, ct);
        if (botUserId is null)
        {
            await _sendUnableToIdentifyMessage.Send(chatId, ct);
            return;
        }

            // Save the answer and show next question
            var success = await _answerService.SaveBooleanAnswerAsync(
                chatId, botUserId.Value, command.QuestionId, true, ct);

            await _answerResultCS.SendAnswerStatus(query, success, ct);


            _logger.LogInformation(
                "User answered 'Yes' to question {QuestionId} in chat {ChatId}",
                command.QuestionId, chatId);
    }
}
