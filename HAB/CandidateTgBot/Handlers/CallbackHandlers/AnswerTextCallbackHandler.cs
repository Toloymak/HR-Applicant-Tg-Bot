using CandidateTgBot.Services;
using CandidateTgBot.Types.Callbacks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public class AnswerTextCallbackHandler : ICallbackHandler<AnswerTextCallback>
{
    private readonly ITelegramBotClient _tg;
    private readonly AnswerService _answerService;
    private readonly BotUserService _botUserService;
    private readonly ILogger<AnswerTextCallbackHandler> _logger;

    public AnswerTextCallbackHandler(
        ITelegramBotClient tg,
        AnswerService answerService,
        BotUserService botUserService,
        ILogger<AnswerTextCallbackHandler> logger)
    {
        _tg = tg;
        _answerService = answerService;
        _botUserService = botUserService;
        _logger = logger;
    }

    public async Task Handle(
        long chatId,
        AnswerTextCallback command,
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

            // Initiate text answer collection
            var success = await _answerService.InitiateTextAnswerAsync(
                chatId, botUserId.Value, command.QuestionId, ct);

            if (success)
            {
                await _tg.AnswerCallbackQuery(
                    callbackQueryId: query.Id,
                    text: "Please type your answer",
                    showAlert: false,
                    cancellationToken: ct
                );
            }
            else
            {
                await _tg.AnswerCallbackQuery(
                    callbackQueryId: query.Id,
                    text: "Error initiating text answer",
                    showAlert: true,
                    cancellationToken: ct
                );
            }

            _logger.LogInformation(
                "User initiated text answer for question {QuestionId} in chat {ChatId}",
                command.QuestionId, chatId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling AnswerTextCallback for question {QuestionId}", command.QuestionId);
            
            await _tg.SendMessage(
                chatId: chatId,
                text: "❌ An error occurred while processing your request. Please try again.",
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
