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
            // Check for active application first
            var activeApplication = await _context.UserApplications
                .Include(a => a.Vacancy)
                .Include(a => a.Answers)
                    .ThenInclude(ans => ans.Question)
                .FirstOrDefaultAsync(a => a.BotUserId == botUserId && 
                                         a.State == ApplicationStatus.InProgress, 
                                         cancellationToken);

            if (activeApplication != null)
            {
                await ShowActiveApplicationStatusAsync(chatId, activeApplication, cancellationToken);
                return;
            }

            // Check for completed applications (both CompletedByUser and CompeatedByUserAndStartedNew)
            var completedApplications = await _context.UserApplications
                .Include(a => a.Vacancy)
                .Where(a => a.BotUserId == botUserId && 
                           (a.State == ApplicationStatus.CompletedByUser || 
                            a.State == ApplicationStatus.CompetedByUserAndStartedNew) &&
                           a.State != ApplicationStatus.RevokedByUser)
                .OrderByDescending(a => a.LastActivity)
                .ToListAsync(cancellationToken);

            if (completedApplications.Any())
            {
                await ShowCompletedApplicationsStatusAsync(chatId, completedApplications, cancellationToken);
                return;
            }

            // Check for completed applications to show short status
            var shortStatusApplications = await _context.UserApplications
                .Include(a => a.Vacancy)
                .Where(a => a.BotUserId == botUserId && 
                           (a.State == ApplicationStatus.CompletedByUser || 
                            a.State == ApplicationStatus.CompetedByUserAndStartedNew) &&
                           a.State != ApplicationStatus.RevokedByUser)
                .OrderByDescending(a => a.LastActivity)
                .ToListAsync(cancellationToken);

            if (shortStatusApplications.Any())
            {
                await ShowShortCompletedStatusAsync(chatId, shortStatusApplications, cancellationToken);
                return;
            }

            // No applications found
            await ShowNoApplicationsMessageAsync(chatId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing application status for user {BotUserId}", botUserId);
            await SendErrorMessageAsync(chatId, cancellationToken);
        }
    }

    /// <summary>
    /// Shows status of an active application with all answered questions
    /// </summary>
    private async Task ShowActiveApplicationStatusAsync(
        long chatId,
        UserApplicationDal application,
        CancellationToken cancellationToken)
    {
        var statusText = $"📋 **Active Application Status**\n\n" +
                        $"**Position:** {application.Vacancy?.Title}\n" +
                        $"**Started:** {application.StartDate:MMM dd, yyyy}\n" +
                        $"**Last Activity:** {application.LastActivity:MMM dd, yyyy HH:mm}\n\n";

        if (application.Answers?.Any() == true)
        {
            statusText += "**📝 Your Answers:**\n\n";
            
            var questions = application.Vacancy?.Questions?.OrderBy(q => q.OrderNumber).ToList() ?? new List<QuestionDal>();
            var answeredQuestions = application.Answers.ToList();

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

        // Add "Revoke Application" button if there are revokable applications
        if (hasRevokableApplications)
        {
            buttons.Add(new[]
            {
                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(
                    text: "🗑️ Revoke Application",
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
                     "💡 *Use /start to browse available positions and begin your first application.*";

        await _tgClient.SendMessage(
            chatId: chatId,
            text: message,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
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
