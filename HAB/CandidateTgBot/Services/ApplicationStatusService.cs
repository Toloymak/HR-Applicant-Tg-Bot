using DataLayer.Contexts;
using DataLayer.Dals;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Models;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace CandidateTgBot.Services;

public class ApplicationStatusService
{
    private readonly HrBotContext _context;
    private readonly ITelegramBotClient _tgClient;
    private readonly ILogger<ApplicationStatusService> _logger;

    public ApplicationStatusService(
        HrBotContext context,
        ITelegramBotClient tgClient,
        ILogger<ApplicationStatusService> logger)
    {
        _context = context;
        _tgClient = tgClient;
        _logger = logger;
    }

        /// <summary>
    /// Shows application status for a user
    /// </summary>
    public async Task ShowApplicationStatusAsync(
        long chatId,
        Guid botUserId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Get all applications for the user (excluding revoked)
            var allApplications = await _context.UserApplications
                .Include(a => a.Vacancy)
                .Include(a => a.Answers)
                    .ThenInclude(ans => ans.Question)
                .Where(a => a.BotUserId == botUserId && 
                           a.State == ApplicationStatus.CompletedByUser
                           || a.State == ApplicationStatus.CompetedByUserAndStartedNew)
                .OrderByDescending(a => a.LastActivity)
                .ToListAsync(cancellationToken);

            if (!allApplications.Any())
            {
                await ShowNoApplicationsMessageAsync(chatId, cancellationToken);
                return;
            }

            // Show all applications in one comprehensive status
            await ShowAllApplicationsStatusAsync(chatId, allApplications, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing application status for user {BotUserId}", botUserId);
            await SendErrorMessageAsync(chatId, cancellationToken);
        }
    }

    /// <summary>
    /// Shows all applications in one comprehensive status
    /// </summary>
    private async Task ShowAllApplicationsStatusAsync(
        long chatId,
        List<UserApplicationDal> applications,
        CancellationToken cancellationToken)
    {
        var statusText = $"📋 **Application Status Overview**\n\n";

        // Group applications by status
        var activeApplications = applications.Where(a => a.State == ApplicationStatus.InProgress).ToList();
        var completedApplications = applications.Where(a => a.State == ApplicationStatus.CompletedByUser).ToList();
        var reviewedApplications = applications.Where(a => a.State == ApplicationStatus.CompetedByUserAndStartedNew).ToList();
        var canceledApplications = applications.Where(a => a.State == ApplicationStatus.CanceledByUser).ToList();

        // Show active applications with details
        if (activeApplications.Any())
        {
            statusText += $"🔄 **Active Applications ({activeApplications.Count})**\n\n";
            
            foreach (var application in activeApplications)
            {
                var questions = application.Vacancy?.Questions?.OrderBy(q => q.OrderNumber).ToList() ?? new List<QuestionDal>();
                var answeredQuestions = application.Answers?.ToList() ?? new List<ApplicationAnswerDal>();
                var allQuestionsAnswered = questions.Count > 0 && 
                                          answeredQuestions.Count == questions.Count &&
                                          questions.All(q => answeredQuestions.Any(a => a.QuestionId == q.Id));

                statusText += $"**📄 {application.Vacancy?.Title}**\n";
                statusText += $"Started: {application.StartDate:MMM dd, yyyy}\n";
                
                if (allQuestionsAnswered)
                {
                    statusText += $"Progress: ✅ All questions completed ({answeredQuestions.Count}/{questions.Count})\n";
                }
                else
                {
                    statusText += $"Progress: 📝 {answeredQuestions.Count}/{questions.Count} questions answered\n";
                }
                statusText += "\n";
            }
        }

        // Show completed applications
        if (completedApplications.Any())
        {
            statusText += $"✅ **Completed Applications ({completedApplications.Count})**\n\n";
            
            foreach (var application in completedApplications)
            {
                statusText += $"**📄 {application.Vacancy?.Title}**\n";
                statusText += $"Completed: {application.LastActivity:MMM dd, yyyy} | Status: ✅ Submitted for Review\n\n";
            }
        }

        // Show reviewed applications
        if (reviewedApplications.Any())
        {
            statusText += $"📋 **Reviewed Applications ({reviewedApplications.Count})**\n\n";
            
            foreach (var application in reviewedApplications)
            {
                statusText += $"**📄 {application.Vacancy?.Title}**\n";
                statusText += $"Completed: {application.LastActivity:MMM dd, yyyy} | Status: ✅ Review by HR\n\n";
            }
        }

        // Show canceled applications
        if (canceledApplications.Any())
        {
            statusText += $"❌ **Canceled Applications ({canceledApplications.Count})**\n\n";
            
            foreach (var application in canceledApplications)
            {
                statusText += $"**📄 {application.Vacancy?.Title}**\n";
                statusText += $"Canceled: {application.LastActivity:MMM dd, yyyy}\n\n";
            }
        }

        // Add action buttons
        var buttons = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[]>();

        // Add "Start New Application" button if there are active applications with all questions answered
        var activeWithAllAnswered = activeApplications.Any(a => {
            var questions = a.Vacancy?.Questions?.OrderBy(q => q.OrderNumber).ToList() ?? new List<QuestionDal>();
            var answeredQuestions = a.Answers?.ToList() ?? new List<ApplicationAnswerDal>();
            return questions.Count > 0 && 
                   answeredQuestions.Count == questions.Count &&
                   questions.All(q => answeredQuestions.Any(ans => ans.QuestionId == q.Id));
        });

        if (activeWithAllAnswered)
        {
            buttons.Add(new[]
            {
                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(
                    text: "🚀 Start New Application",
                    callbackData: new CandidateTgBot.Types.Callbacks.StartNewApplicationCallback()
                        .ToTgString().ToString()
                )
            });
        }

        // Add "Revoke Applications" button if there are revokable applications
        var hasRevokableApplications = completedApplications.Any() || reviewedApplications.Any();
        if (hasRevokableApplications)
        {
            buttons.Add(new[]
            {
                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(
                    text: "🗑️ Revoke Applications",
                    callbackData: new CandidateTgBot.Types.Callbacks.RevokeApplicationListCallback()
                        .ToTgString().ToString()
                )
            });
        }

        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup? keyboard = null;
        if (buttons.Any())
        {
            keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(buttons);
        }

        await _tgClient.SendMessage(
            chatId: chatId,
            text: statusText,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: keyboard,
            cancellationToken: cancellationToken
        );
    }

    /// <summary>
    /// Shows status of an active application with all answered questions
    /// </summary>
    private async Task ShowActiveApplicationStatusAsync(
        long chatId,
        UserApplicationDal application,
        CancellationToken cancellationToken)
    {
        var questions = application.Vacancy?.Questions?.OrderBy(q => q.OrderNumber).ToList() ?? new List<QuestionDal>();
        var answeredQuestions = application.Answers?.ToList() ?? new List<ApplicationAnswerDal>();
        
        // Check if all questions are answered
        var allQuestionsAnswered = questions.Count > 0 && 
                                  answeredQuestions.Count == questions.Count &&
                                  questions.All(q => answeredQuestions.Any(a => a.QuestionId == q.Id));

        if (allQuestionsAnswered)
        {
            // Show simplified status when all questions are answered
            var statusText = $"📋 **Active Application Status**\n\n" +
                            $"**Position:** {application.Vacancy?.Title}\n" +
                            $"**Started:** {application.StartDate:MMM dd, yyyy}\n" +
                            $"**Last Activity:** {application.LastActivity:MMM dd, yyyy HH:mm}\n" +
                            $"**Progress:** ✅ All questions completed ({answeredQuestions.Count}/{questions.Count})\n\n" +
                            $"💡 *Your application is ready for submission. Use /continue to complete the process.*";

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
                text: statusText,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: cancellationToken
            );
        }
        else
        {
            // Show detailed status with questions and answers
            var statusText = $"📋 **Active Application Status**\n\n" +
                            $"**Position:** {application.Vacancy?.Title}\n" +
                            $"**Started:** {application.StartDate:MMM dd, yyyy}\n" +
                            $"**Last Activity:** {application.LastActivity:MMM dd, yyyy HH:mm}\n\n";

            if (answeredQuestions.Any())
            {
                statusText += "**📝 Your Answers:**\n\n";
                
                foreach (var question in questions)
                {
                    var answer = answeredQuestions.FirstOrDefault(a => a.QuestionId == question.Id);
                    
                    if (answer != null)
                    {
                        var answerText = GetAnswerDisplayText(answer);
                        statusText += $"**Q{question.OrderNumber}:** {question.Text}\n" +
                                    $"**A:** {answerText}\n\n";
                    }
                    else
                    {
                        statusText += $"**Q{question.OrderNumber}:** {question.Text}\n" +
                                    $"**A:** *Not answered yet*\n\n";
                    }
                }
            }
            else
            {
                statusText += "**📝 Your Answers:**\n\n" +
                             "*No questions answered yet.*\n\n";
            }

            statusText += "💡 *Use /continue to resume your application or /cancel to start over.*";

            await _tgClient.SendMessage(
                chatId: chatId,
                text: statusText,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                cancellationToken: cancellationToken
            );
        }

        _logger.LogInformation(
            "Showed active application status for user {BotUserId}, application {ApplicationId}",
            application.BotUserId, application.Id);
    }

    /// <summary>
    /// Shows list of completed applications
    /// </summary>
    private async Task ShowCompletedApplicationsStatusAsync(
        long chatId,
        List<UserApplicationDal> applications,
        CancellationToken cancellationToken)
    {
        var statusText = $"✅ **Completed Applications**\n\n" +
                        $"You have {applications.Count} completed application(s):\n\n";

        foreach (var application in applications)
        {
            var completionDate = application.LastActivity.ToString("MMM dd, yyyy");
            var statusDisplay = application.State switch
            {
                ApplicationStatus.CompletedByUser => "✅ Submitted for Review",
                ApplicationStatus.CompetedByUserAndStartedNew => "✅ Review by HR",
                _ => "Unknown Status"
            };
            
            statusText += $"**📄 {application.Vacancy?.Title}**\n" +
                         $"Completed: {completionDate}\n" +
                         $"Status: {statusDisplay}\n\n";
        }

        statusText += "💡 *Your applications have been submitted and are being reviewed by our HR team.*\n\n" +
                     "*You will be notified about the status of your applications.*";

        // Check if all applications are in review and if any can be revoked
        var allInReview = applications.All(a => 
            a.State == ApplicationStatus.CompletedByUser || 
            a.State == ApplicationStatus.CompetedByUserAndStartedNew);
        
        var hasRevokableApplications = applications.Any(a => 
            a.State == ApplicationStatus.CompletedByUser ||
            a.State == ApplicationStatus.CompetedByUserAndStartedNew);

        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup? keyboard = null;
        var buttons = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[]>();

        // Add "Start new" button if all applications are in review
        if (allInReview)
        {
            buttons.Add(new[]
            {
                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(
                    text: "🚀 Start New Application",
                    callbackData: new CandidateTgBot.Types.Callbacks.StartNewApplicationCallback()
                        .ToTgString().ToString()
                )
            });
        }

        // Add "Revoke Applications" button if there are revokable applications
        if (hasRevokableApplications)
        {
            buttons.Add(new[]
            {
                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(
                    text: "🗑️ Revoke Applications",
                    callbackData: new CandidateTgBot.Types.Callbacks.RevokeApplicationListCallback()
                        .ToTgString().ToString()
                )
            });
        }

        if (buttons.Any())
        {
            keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(buttons);
        }

        await _tgClient.SendMessage(
            chatId: chatId,
            text: statusText,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: keyboard,
            cancellationToken: cancellationToken
        );

        _logger.LogInformation(
            "Showed completed applications status for user, {Count} applications",
            applications.Count);
    }

    /// <summary>
    /// Shows other completed applications as a separate block (when user has active application)
    /// </summary>
    private async Task ShowOtherCompletedApplicationsBlockAsync(
        long chatId,
        List<UserApplicationDal> applications,
        CancellationToken cancellationToken)
    {
        var statusText = $"📋 **Other Applications**\n\n" +
                        $"You also have {applications.Count} other completed application(s):\n\n";

        foreach (var application in applications.Take(3)) // Show only first 3
        {
            var completionDate = application.LastActivity.ToString("MMM dd, yyyy");
            var statusDisplay = application.State switch
            {
                ApplicationStatus.CompletedByUser => "✅ Submitted for Review",
                ApplicationStatus.CompetedByUserAndStartedNew => "✅ Review by HR",
                _ => "Unknown Status"
            };
            
            statusText += $"**📄 {application.Vacancy?.Title}**\n" +
                         $"Completed: {completionDate} | Status: {statusDisplay}\n\n";
        }

        if (applications.Count > 3)
        {
            statusText += $"*... and {applications.Count - 3} more application(s)*\n\n";
        }

        // Check if any applications can be revoked
        var hasRevokableApplications = applications.Any(a => 
            a.State == ApplicationStatus.CompletedByUser ||
            a.State == ApplicationStatus.CompetedByUserAndStartedNew);

        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup? keyboard = null;
        if (hasRevokableApplications)
        {
            keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(
                        text: "🗑️ Revoke Applications",
                        callbackData: new CandidateTgBot.Types.Callbacks.RevokeApplicationListCallback()
                            .ToTgString().ToString()
                    )
                }
            });
        }

        await _tgClient.SendMessage(
            chatId: chatId,
            text: statusText,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: keyboard,
            cancellationToken: cancellationToken
        );
    }

    /// <summary>
    /// Shows completed applications status with start new button
    /// </summary>
    private async Task ShowCompletedApplicationsForStartNewAsync(
        long chatId,
        List<UserApplicationDal> applications,
        CancellationToken cancellationToken)
    {
        var statusText = $"📋 **Completed Applications**\n\n" +
                        $"You have {applications.Count} completed application(s):\n\n";

        foreach (var application in applications.Take(3)) // Show only first 3
        {
            var completionDate = application.LastActivity.ToString("MMM dd, yyyy");
            statusText += $"**📄 {application.Vacancy?.Title}**\n" +
                         $"Completed: {completionDate} | Status: ✅ Submitted for Review\n\n";
        }

        if (applications.Count > 3)
        {
            statusText += $"*... and {applications.Count - 3} more completed application(s)*\n\n";
        }

        statusText += "💡 *Ready to start a new application?*";

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
            text: statusText,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: keyboard,
            cancellationToken: cancellationToken
        );
    }

    /// <summary>
    /// Shows short status of completed applications with start new button
    /// </summary>
    private async Task ShowShortCompletedStatusAsync(
        long chatId,
        List<UserApplicationDal> applications,
        CancellationToken cancellationToken)
    {
        var statusText = $"📋 **No Active Applications**\n\n" +
                        $"You have {applications.Count} completed application(s):\n\n";

        foreach (var application in applications.Take(3)) // Show only first 3
        {
            var completionDate = application.LastActivity.ToString("MMM dd, yyyy");
            var statusDisplay = application.State switch
            {
                ApplicationStatus.CompletedByUser => "✅ Submitted for Review",
                ApplicationStatus.CompetedByUserAndStartedNew => "✅ Review by HR",
                _ => "Unknown Status"
            };
            
            statusText += $"**📄 {application.Vacancy?.Title}**\n" +
                         $"Completed: {completionDate} | Status: {statusDisplay}\n\n";
        }

        if (applications.Count > 3)
        {
            statusText += $"*... and {applications.Count - 3} more application(s)*\n\n";
        }

        statusText += "💡 *Ready to start a new application?*";

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
            text: statusText,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: keyboard,
            cancellationToken: cancellationToken
        );
    }

    /// <summary>
    /// Shows message when user has no applications
    /// </summary>
    private async Task ShowNoApplicationsMessageAsync(long chatId, CancellationToken cancellationToken)
    {
        var message = "📭 **No Applications Found**\n\n" +
                     "You don't have any applications yet.\n\n" +
                     "💡 *Ready to start your first application?*";

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

    /// <summary>
    /// Converts answer value to display text
    /// </summary>
    private string GetAnswerDisplayText(ApplicationAnswerDal answer)
    {
        return answer.AnswerValue switch
        {
            DataLayer.Dals.BooleanAnswerValue boolAnswer => boolAnswer.Value ? "✅ Yes" : "❌ No",
            DataLayer.Dals.TextAnswerValue textAnswer => textAnswer.Value,
            _ => "*Unknown answer type*"
        };
    }

    /// <summary>
    /// Sends error message
    /// </summary>
    private async Task SendErrorMessageAsync(long chatId, CancellationToken cancellationToken)
    {
        await _tgClient.SendMessage(
            chatId: chatId,
            text: "❌ **Error Loading Status**\n\n" +
                  "An error occurred while loading your application status.\n\n" +
                  "💡 *Please try again later or contact support if the issue persists.*",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            cancellationToken: cancellationToken
        );
    }
}
