using CandidateTgBot.Services;
using CandidateTgBot.Types.Callbacks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public class AnswerNoCallbackHandler : ICallbackHandler<AnswerNoCallback>
{
    private readonly ITelegramBotClient _tg;
    private readonly AnswerService _answerService;
    private readonly BotUserService _botUserService;
    private readonly ILogger<AnswerNoCallbackHandler> _logger;

    public AnswerNoCallbackHandler(
        ITelegramBotClient tg,
        AnswerService answerService,
        BotUserService botUserService,
        ILogger<AnswerNoCallbackHandler> logger)
    {
        _tg = tg;
        _answerService = answerService;
        _botUserService = botUserService;
        _logger = logger;
    }

    public async Task Handle(
        long chatId,
        AnswerNoCallback command,
        CallbackQuery query,
        CancellationToken ct)
    {
        try
        {
            // Get bot user ID from the callback query
            var botUserId = await GetBotUserIdFromCallback(query, ct);
            if (botUserId == null)
            {
                await _tg.SendMessage(
                    chatId: chatId,
                    text: "❌ Unable to identify your user account. Please try again.",
                    cancellationToken: ct
                );
                return;
            }

            // Save the answer and show next question
            var success = await _answerService.SaveBooleanAnswerAsync(
                chatId, botUserId.Value, command.QuestionId, false, ct);

            if (success)
            {
                await _tg.AnswerCallbackQuery(
                    callbackQueryId: query.Id,
                    text: "Answer saved!",
                    showAlert: false,
                    cancellationToken: ct
                );
            }
            else
            {
                await _tg.AnswerCallbackQuery(
                    callbackQueryId: query.Id,
                    text: "Error saving answer",
                    showAlert: true,
                    cancellationToken: ct
                );
            }

            _logger.LogInformation(
                "User answered 'No' to question {QuestionId} in chat {ChatId}",
                command.QuestionId, chatId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling AnswerNoCallback for question {QuestionId}", command.QuestionId);
            
            await _tg.SendMessage(
                chatId: chatId,
                text: "❌ An error occurred while processing your answer. Please try again.",
                cancellationToken: ct
            );
        }
    }

    private async Task<Guid?> GetBotUserIdFromCallback(CallbackQuery query, CancellationToken ct)
    {
        if (query.From == null) return null;
        
        try
        {
            var botUser = await _botUserService.CreateOrUpdateUserAsync(query.From, ct);
            return botUser?.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bot user for Telegram user {TgId}", query.From.Id);
            return null;
        }
    }
}
