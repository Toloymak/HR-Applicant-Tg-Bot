using DataLayer.Contexts;
using DataLayer.Dals;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Models;
using Telegram.Bot;

namespace CandidateTgBot.Services;

public class AnswerService
{
    private readonly HrBotContext _context;
    private readonly CurrentQuestionService _questionService;
    private readonly ITelegramBotClient _tgClient;
    private readonly ILogger<AnswerService> _logger;

    public AnswerService(
        HrBotContext context,
        CurrentQuestionService questionService,
        ITelegramBotClient tgClient,
        ILogger<AnswerService> logger)
    {
        _context = context;
        _questionService = questionService;
        _tgClient = tgClient;
        _logger = logger;
    }

    /// <summary>
    /// Saves a boolean answer and shows the next question
    /// </summary>
    public async Task<bool> SaveBooleanAnswerAsync(
        long chatId,
        Guid botUserId,
        Guid questionId,
        bool answer,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Find the active application for this user
            var application = await _context.UserApplications
                .Include(a => a.Vacancy)
                .FirstOrDefaultAsync(a => a.BotUserId == botUserId && 
                                         a.State == ApplicationStatus.InProgress, 
                                         cancellationToken);

            if (application == null)
            {
                _logger.LogWarning("No active application found for user {BotUserId}", botUserId);
                await SendNoActiveApplicationMessageAsync(chatId, cancellationToken);
                return false;
            }

            // Get the question to validate it exists
            var question = await _context.Questions
                .FirstOrDefaultAsync(q => q.Id == questionId && q.VacancyId == application.VacancyId, 
                                    cancellationToken);

            if (question == null)
            {
                _logger.LogWarning("Question {QuestionId} not found for vacancy {VacancyId}", 
                    questionId, application.VacancyId);
                await SendQuestionNotFoundMessageAsync(chatId, cancellationToken);
                return false;
            }

            // Check if this question was already answered
            var existingAnswer = await _context.Answers
                .FirstOrDefaultAsync(a => a.UserApplicationId == application.Id && 
                                         a.QuestionId == questionId, 
                                         cancellationToken);

            if (existingAnswer != null)
            {
                _logger.LogInformation("Question {QuestionId} already answered for application {ApplicationId}", 
                    questionId, application.Id);
                // Continue to next question anyway
            }
            else
            {
                // Create new answer
                var answerDal = new ApplicationAnswerDal
                {
                    UserApplicationId = application.Id,
                    QuestionId = questionId,
                    AnswerValue = new DataLayer.Dals.BooleanAnswerValue { Value = answer }
                };

                _context.Answers.Add(answerDal);
            }

            // Update application's last question and activity
            application.LastQuestionId = questionId;
            application.LastActivity = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Saved boolean answer {Answer} for question {QuestionId} in application {ApplicationId}",
                answer, questionId, application.Id);

            // Show next question
            await _questionService.SendCurrentQuestionAsync(chatId, application.Id, botUserId, cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving boolean answer for question {QuestionId}", questionId);
            await SendErrorMessageAsync(chatId, cancellationToken);
            return false;
        }
    }

    /// <summary>
    /// Initiates text answer collection
    /// </summary>
    public async Task<bool> InitiateTextAnswerAsync(
        long chatId,
        Guid botUserId,
        Guid questionId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Find the active application for this user
            var application = await _context.UserApplications
                .Include(a => a.Vacancy)
                .FirstOrDefaultAsync(a => a.BotUserId == botUserId && 
                                         a.State == ApplicationStatus.InProgress, 
                                         cancellationToken);

            if (application == null)
            {
                _logger.LogWarning("No active application found for user {BotUserId}", botUserId);
                await SendNoActiveApplicationMessageAsync(chatId, cancellationToken);
                return false;
            }

            // Get the question
            var question = await _context.Questions
                .FirstOrDefaultAsync(q => q.Id == questionId && q.VacancyId == application.VacancyId, 
                                    cancellationToken);

            if (question == null)
            {
                _logger.LogWarning("Question {QuestionId} not found for vacancy {VacancyId}", 
                    questionId, application.VacancyId);
                await SendQuestionNotFoundMessageAsync(chatId, cancellationToken);
                return false;
            }

            // Update application's last question to indicate we're waiting for text input
            application.LastQuestionId = questionId;
            application.LastActivity = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            // Send message asking for text input
            await SendTextInputRequestAsync(chatId, question, cancellationToken);

            _logger.LogInformation(
                "Initiated text answer collection for question {QuestionId} in application {ApplicationId}",
                questionId, application.Id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initiating text answer for question {QuestionId}", questionId);
            await SendErrorMessageAsync(chatId, cancellationToken);
            return false;
        }
    }

    /// <summary>
    /// Saves a text answer when user provides it via message
    /// </summary>
    public async Task<bool> SaveTextAnswerAsync(
        long chatId,
        Guid botUserId,
        string textAnswer,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Find the active application for this user
            var application = await _context.UserApplications
                .Include(a => a.Vacancy)
                .FirstOrDefaultAsync(a => a.BotUserId == botUserId && 
                                         a.State == ApplicationStatus.InProgress, 
                                         cancellationToken);

            if (application == null)
            {
                _logger.LogWarning("No active application found for user {BotUserId}", botUserId);
                await SendNoActiveApplicationMessageAsync(chatId, cancellationToken);
                return false;
            }

            // Get the last question that was asked
            if (application.LastQuestionId == null)
            {
                _logger.LogWarning("No last question found for application {ApplicationId}", application.Id);
                await SendNoQuestionInProgressMessageAsync(chatId, cancellationToken);
                return false;
            }

            var question = await _context.Questions
                .FirstOrDefaultAsync(q => q.Id == application.LastQuestionId.Value && 
                                         q.VacancyId == application.VacancyId, 
                                         cancellationToken);

            if (question == null)
            {
                _logger.LogWarning("Question {QuestionId} not found for vacancy {VacancyId}", 
                    application.LastQuestionId.Value, application.VacancyId);
                await SendQuestionNotFoundMessageAsync(chatId, cancellationToken);
                return false;
            }

            // Check if this question was already answered
            var existingAnswer = await _context.Answers
                .FirstOrDefaultAsync(a => a.UserApplicationId == application.Id && 
                                         a.QuestionId == question.Id, 
                                         cancellationToken);

            if (existingAnswer != null)
            {
                _logger.LogInformation("Question {QuestionId} already answered for application {ApplicationId}", 
                    question.Id, application.Id);
                // Continue to next question anyway
            }
            else
            {
                // Create new answer
                var answerDal = new ApplicationAnswerDal
                {
                    UserApplicationId = application.Id,
                    QuestionId = question.Id,
                    AnswerValue = new DataLayer.Dals.TextAnswerValue { Value = textAnswer }
                };

                _context.Answers.Add(answerDal);
            }

            // Update application activity
            application.LastActivity = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Saved text answer for question {QuestionId} in application {ApplicationId}",
                question.Id, application.Id);

            // Show next question
            await _questionService.SendCurrentQuestionAsync(chatId, application.Id, botUserId, cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving text answer for user {BotUserId}", botUserId);
            await SendErrorMessageAsync(chatId, cancellationToken);
            return false;
        }
    }

    private async Task SendNoActiveApplicationMessageAsync(long chatId, CancellationToken cancellationToken)
    {
        var message = "❌ **No Active Application**\n\n" +
                      "You don't have an active application to answer questions for.\n\n" +
                      "💡 *Ready to start a new application?*";

        var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
        {
            new[]
            {
                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(
                    text: "🚀 Start New Application",
                    callbackData: new CandidateTgBot.Types.Callbacks.StartNewApplicationCallback()
                        .ToTgString().ToString()
                )
            }
        });

        await _tgClient.SendMessage(
            chatId: chatId,
            text: message,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: keyboard,
            cancellationToken: cancellationToken
        );
    }

    private async Task SendQuestionNotFoundMessageAsync(long chatId, CancellationToken cancellationToken)
    {
        await _tgClient.SendMessage(
            chatId: chatId,
            text: "❌ **Question Not Found**\n\n" +
                  "The question you're trying to answer could not be found.\n\n" +
                  "💡 *Use /continue to resume your application.*",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            cancellationToken: cancellationToken
        );
    }

    private async Task SendNoQuestionInProgressMessageAsync(long chatId, CancellationToken cancellationToken)
    {
        await _tgClient.SendMessage(
            chatId: chatId,
            text: "❌ **No Question in Progress**\n\n" +
                  "There's no question currently waiting for your answer.\n\n" +
                  "💡 *Use /continue to resume your application.*",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            cancellationToken: cancellationToken
        );
    }

    private async Task SendErrorMessageAsync(long chatId, CancellationToken cancellationToken)
    {
        await _tgClient.SendMessage(
            chatId: chatId,
            text: "❌ **Error Processing Answer**\n\n" +
                  "An error occurred while processing your answer.\n\n" +
                  "💡 *Please try again or use /continue to resume your application.*",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            cancellationToken: cancellationToken
        );
    }

    private async Task SendTextInputRequestAsync(long chatId, QuestionDal question, CancellationToken cancellationToken)
    {
        await _tgClient.SendMessage(
            chatId: chatId,
            text: $"✏️ **Text Answer Required**\n\n" +
                  $"**Question:** {question.Text}\n\n" +
                  $"Please type your answer below and send it as a message.\n\n" +
                  $"💡 *Your answer should be detailed and relevant to the question.*",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            cancellationToken: cancellationToken
        );
    }
}
