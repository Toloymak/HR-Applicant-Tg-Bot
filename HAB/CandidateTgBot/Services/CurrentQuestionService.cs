using DataLayer.Contexts;
using DataLayer.Dals;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Models;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using CandidateTgBot.Types;
using CandidateTgBot.Types.Callbacks;

namespace CandidateTgBot.Services;

public class CurrentQuestionService
{
    private readonly HrBotContext _context;
    private readonly ITelegramBotClient _tgClient;
    private readonly CancelApplicationButtonService _cancelButtonService;
    private readonly ILogger<CurrentQuestionService> _logger;

    public CurrentQuestionService(
        HrBotContext context,
        ITelegramBotClient tgClient,
        CancelApplicationButtonService cancelButtonService,
        ILogger<CurrentQuestionService> logger)
    {
        _context = context;
        _tgClient = tgClient;
        _cancelButtonService = cancelButtonService;
        _logger = logger;
    }

    /// <summary>
    /// Sends the current question for an active application
    /// </summary>
    /// <param name="chatId">Chat ID to send the question to</param>
    /// <param name="applicationId">ID of the active application</param>
    /// <param name="botUserId">Bot user ID for cancel button</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if question was sent, false if no question available</returns>
    public async Task<bool> SendCurrentQuestionAsync(
        long chatId,
        Guid applicationId,
        Guid botUserId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Get the application with related data
            var application = await _context.UserApplications
                .Include(a => a.Vacancy)
                    .ThenInclude(v => v!.Questions)
                .Include(a => a.LastQuestion)
                .Include(a => a.Answers)
                .FirstOrDefaultAsync(a => a.Id == applicationId, cancellationToken);

            if (application?.Vacancy == null)
            {
                _logger.LogWarning("Application {ApplicationId} not found or has no vacancy", applicationId);
                return false;
            }

            // Determine the current question
            var currentQuestion = await GetCurrentQuestionAsync(application, cancellationToken);

            if (currentQuestion == null)
            {
                // No more questions - application might be complete
                await SendApplicationCompleteMessageAsync(chatId, application, botUserId, cancellationToken);
                return true;
            }

            // Send the current question
            await SendQuestionAsync(chatId, currentQuestion, application, botUserId, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending current question for application {ApplicationId}", applicationId);
            
            await _tgClient.SendMessage(
                chatId: chatId,
                text: "❌ An error occurred while loading your question. Please try again later.",
                cancellationToken: cancellationToken
            );
            
            return false;
        }
    }

    /// <summary>
    /// Determines the current question for an application
    /// </summary>
    private Task<QuestionDal?> GetCurrentQuestionAsync(
        UserApplicationDal application, 
        CancellationToken cancellationToken)
    {
        var vacancy = application.Vacancy!;
        var questions = vacancy.Questions?.OrderBy(q => q.OrderNumber).ToList() ?? new List<QuestionDal>();

        if (!questions.Any())
        {
            _logger.LogWarning("Vacancy {VacancyId} has no questions", vacancy.Id);
            return Task.FromResult<QuestionDal?>(null);
        }

        // If no questions answered yet, return the first question
        if (application.LastQuestionId == null)
        {
            return Task.FromResult<QuestionDal?>(questions.First());
        }

        // Get answered questions
        var answeredQuestions = application.Answers?.Select(a => a.QuestionId).ToHashSet() ?? new HashSet<Guid>();

        // Find the next unanswered question
        var nextQuestion = questions.FirstOrDefault(q => !answeredQuestions.Contains(q.Id));

        return Task.FromResult(nextQuestion);
    }

    /// <summary>
    /// Sends a question to the user
    /// </summary>
    private async Task SendQuestionAsync(
        long chatId,
        QuestionDal question,
        UserApplicationDal application,
        Guid botUserId,
        CancellationToken cancellationToken)
    {
        var keyboard = await CreateQuestionKeyboardAsync(question, botUserId, cancellationToken);

        var questionText = $"📋 **{application.Vacancy!.Title} - Application**\n\n" +
                          $"❓ **Question {GetQuestionNumber(application, question)}:**\n\n" +
                          $"{question.Text}";

        await _tgClient.SendMessage(
            chatId: chatId,
            text: questionText,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: keyboard,
            cancellationToken: cancellationToken
        );

        _logger.LogInformation(
            "Sent question {QuestionId} for application {ApplicationId} in chat {ChatId}",
            question.Id, application.Id, chatId);
    }

    /// <summary>
    /// Creates keyboard for a question based on its answer type
    /// </summary>
    private async Task<InlineKeyboardMarkup> CreateQuestionKeyboardAsync(
        QuestionDal question,
        Guid botUserId,
        CancellationToken cancellationToken)
    {
        var buttons = new List<List<InlineKeyboardButton>>();

        // Add answer buttons based on question type
        switch (question.Answer.Type)
        {
            case YesNoAnswerTypeDal.TypeName:
            case "yesno_required":
                buttons.Add(new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData("✅ Yes", $"answer_yes|{GuidShort.ToBase64Url(question.Id)}"),
                    InlineKeyboardButton.WithCallbackData("❌ No", $"answer_no|{GuidShort.ToBase64Url(question.Id)}")
                });
                break;

            case TextAnswerTypeDal.TypeName:
                buttons.Add(new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData("✏️ Provide Text Answer", $"answer_text|{GuidShort.ToBase64Url(question.Id)}")
                });
                break;
        }

        // Add cancel button
        var cancelKeyboard = await _cancelButtonService.CreateCancelButtonKeyboardAsync(botUserId, cancellationToken);
        if (cancelKeyboard != null)
        {
            buttons.AddRange(cancelKeyboard.InlineKeyboard.Select(row => row.ToList()));
        }

        return new InlineKeyboardMarkup(buttons);
    }

    /// <summary>
    /// Gets the question number in the sequence
    /// </summary>
    private int GetQuestionNumber(UserApplicationDal application, QuestionDal currentQuestion)
    {
        var questions = application.Vacancy!.Questions?.OrderBy(q => q.OrderNumber).ToList() ?? new List<QuestionDal>();
        var index = questions.FindIndex(q => q.Id == currentQuestion.Id);
        return index + 1;
    }

    /// <summary>
    /// Sends application complete message
    /// </summary>
    private async Task SendApplicationCompleteMessageAsync(
        long chatId,
        UserApplicationDal application,
        Guid botUserId,
        CancellationToken cancellationToken)
    {
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    text: "📝 Send One More Application",
                    callbackData: new SendOneMoreApplicationCallback
                    {
                        CompletedApplicationId = application.Id
                    }.ToTgString().ToString()
                )
            }
        });

        await _tgClient.SendMessage(
            chatId: chatId,
            text: $"🎉 **Application Complete!**\n\n" +
                  $"You have successfully completed your application for **{application.Vacancy!.Title}**.\n\n" +
                  $"✅ Your application has been submitted and will be reviewed by our HR team.\n\n" +
                  $"💡 *Use /status to check your application status anytime.*\n\n" +
                  $"*You will be notified about the status of your application.*",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: keyboard,
            cancellationToken: cancellationToken
        );

        _logger.LogInformation(
            "Application {ApplicationId} completed for user in chat {ChatId}",
            application.Id, chatId);
    }
}
