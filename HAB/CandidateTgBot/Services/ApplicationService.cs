using DataLayer.Contexts;
using DataLayer.Dals;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Models;
using Telegram.Bot;

namespace CandidateTgBot.Services;

public class ApplicationService
{
    private readonly HrBotContext _context;
    private readonly ILogger<ApplicationService> _logger;

    public ApplicationService(HrBotContext context, ILogger<ApplicationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new application for a user applying to a vacancy
    /// </summary>
    /// <param name="botUserId">The ID of the BotUser applying</param>
    /// <param name="vacancyId">The ID of the vacancy being applied to</param>
    /// <param name="chatId">The Telegram Chat ID for sending messages to the user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created UserApplicationDal</returns>
    public async Task<UserApplicationDal> CreateApplicationAsync(
        Guid botUserId, 
        Guid vacancyId, 
        long chatId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        // Verify vacancy exists and is active
        var vacancy = await _context.Vacancies
            .FirstOrDefaultAsync(v => 
                v.Id == vacancyId 
                && v.IsActive,
                cancellationToken);

        if (vacancy == null)
        {
            throw new InvalidOperationException(
                $"Vacancy {vacancyId} not found or not active");
        }

        // Create new application
        var application = new UserApplicationDal
        {
            Id = Guid.NewGuid(),
            BotUserId = botUserId,
            VacancyId = vacancyId,
            ChatId = chatId,
            State = ApplicationStatus.InProgress,
            StartDate = now,
            LastActivity = now,
            LastQuestionId = null // Will be set when questions are started
        };

        _context.UserApplications.Add(application);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Created new application {ApplicationId} for user {BotUserId} applying to vacancy '{VacancyTitle}' ({VacancyId})",
            application.Id, botUserId, vacancy.Title, vacancyId);

        return application;
    }

    /// <summary>
    /// Updates the last activity timestamp for an application
    /// </summary>
    /// <param name="applicationId">Application ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task UpdateApplicationActivityAsync(Guid applicationId, CancellationToken cancellationToken = default)
    {
        var application = await _context.UserApplications
            .FirstOrDefaultAsync(a => a.Id == applicationId, cancellationToken);

        if (application != null)
        {
            application.LastActivity = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("Updated activity for application {ApplicationId}", applicationId);
        }
    }

    /// <summary>
    /// Updates application status
    /// </summary>
    /// <param name="applicationId">Application ID</param>
    /// <param name="newStatus">New status</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task UpdateApplicationStatus(
        Guid applicationId, 
        ApplicationStatus newStatus, 
        CancellationToken cancellationToken = default)
    {
        var application = await _context.UserApplications
            .FirstOrDefaultAsync(a => a.Id == applicationId, cancellationToken);

        if (application != null)
        {
            var oldStatus = application.State;
            application.State = newStatus;
            application.LastActivity = DateTime.UtcNow;
            
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Updated application {ApplicationId} status from {OldStatus} to {NewStatus}",
                applicationId, oldStatus, newStatus);
        }
    }

    /// <summary>
    /// Gets user's active applications (excluding canceled ones)
    /// </summary>
    /// <param name="botUserId">Bot user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of active applications</returns>
    public async Task<List<UserApplicationDal>> GetUserActiveApplicationsAsync(
        Guid botUserId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.UserApplications
            .Include(a => a.Vacancy)
            .Where(a => a.BotUserId == botUserId && 
                       a.State == ApplicationStatus.InProgress)
            .OrderByDescending(a => a.LastActivity)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Sends a status update message to the user about their application
    /// </summary>
    /// <param name="applicationId">Application ID</param>
    /// <param name="message">Message to send</param>
    /// <param name="telegramClient">Telegram bot client</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task SendStatusUpdateToUserAsync(
        Guid applicationId,
        string message,
        ITelegramBotClient telegramClient,
        CancellationToken cancellationToken = default)
    {
        var application = await _context.UserApplications
            .FirstOrDefaultAsync(a => a.Id == applicationId, cancellationToken);

        if (application == null)
        {
            _logger.LogWarning("Application {ApplicationId} not found for status update", applicationId);
            return;
        }

        try
        {
            await telegramClient.SendMessage(
                chatId: application.ChatId,
                text: message,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                cancellationToken: cancellationToken
            );

            _logger.LogInformation(
                "Sent status update to user for application {ApplicationId} in chat {ChatId}",
                applicationId, application.ChatId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Failed to send status update to user for application {ApplicationId} in chat {ChatId}",
                applicationId, application.ChatId);
        }
    }

    /// <summary>
    /// Gets application by ID with related data
    /// </summary>
    /// <param name="applicationId">Application ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Application with related data or null if not found</returns>
    public async Task<UserApplicationDal?> GetApplicationByIdAsync(
        Guid applicationId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.UserApplications
            .Include(a => a.BotUser)
            .Include(a => a.Vacancy)
            .Include(a => a.LastQuestion)
            .Include(a => a.Answers)
            .FirstOrDefaultAsync(a => a.Id == applicationId, cancellationToken);
    }

    /// <summary>
    /// Checks if user has already applied to a specific vacancy (excluding canceled and revoked applications)
    /// </summary>
    /// <param name="botUserId">Bot user ID</param>
    /// <param name="vacancyId">Vacancy ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>True if user has already applied</returns>
    public async Task<bool> HasUserAppliedToVacancyAsync(
        Guid botUserId, 
        Guid vacancyId, 
        CancellationToken ct = default)
    {
        ApplicationStatus[] activeState = [
            ApplicationStatus.InProgress,
            ApplicationStatus.ReviewByHr
        ];
        
        return await _context.UserApplications
            .AnyAsync(a => a.BotUserId == botUserId && 
                          a.VacancyId == vacancyId && 
                          activeState.Contains(a.State),
                ct);
    }

    /// <summary>
    /// Checks if user has any active applications
    /// </summary>
    /// <param name="botUserId">Bot user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user has active applications</returns>
    public async Task<bool> HasUserActiveApplicationsAsync(
        Guid botUserId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.UserApplications
            .AnyAsync(a => a.BotUserId == botUserId && 
                           a.State == ApplicationStatus.InProgress, 
                     cancellationToken);
    }

    /// <summary>
    /// Gets the most recent active application for a user
    /// </summary>
    /// <param name="botUserId">Bot user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Most recent active application or null</returns>
    public async Task<UserApplicationDal?> GetUserMostRecentActiveApplicationAsync(
        Guid botUserId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.UserApplications
            .Include(a => a.Vacancy)
            .Where(a => a.BotUserId == botUserId && 
                       (a.State == ApplicationStatus.InProgress))
            .OrderByDescending(a => a.LastActivity)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
