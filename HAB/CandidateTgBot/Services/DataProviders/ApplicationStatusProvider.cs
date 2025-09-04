using DataLayer.Contexts;
using DataLayer.Dals;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace CandidateTgBot.Services.DataProviders;

public class ApplicationStatusProvider
{
    private readonly HrBotContext _context;
    
    private static readonly HashSet<ApplicationStatus> CompletedStatusesAvailableToUser =
    [
        ApplicationStatus.ReviewByHr,
        ApplicationStatus.Approved,
        ApplicationStatus.Rejected
    ];
    
    private static readonly TimeSpan MaxApplicationAge = TimeSpan.FromDays(30);

    public ApplicationStatusProvider(
        HrBotContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<UserApplicationStatus>>
        GetApplicationForStatusCheck(
            long chatId,
            Guid botUserId,
            CancellationToken ct)
    {
        var handlingApplications =
            await GetAppToCheckStatusQuery(chatId, botUserId)
                .Select(x => new UserApplicationStatus
                {
                    Id = x.VacancyId,
                    VacancyTitle = x.Vacancy!.Title,
                    State = x.State,
                    StartDate = x.StartDate,
                    LastActivity = x.LastActivity,
                })
                .ToListAsync(ct);

        return handlingApplications;
    }

    public async Task<bool> HasApplicationForStatusCheck(
            long chatId,
            Guid botUserId,
            CancellationToken ct)
        => await GetAppToCheckStatusQuery(chatId, botUserId)
            .AnyAsync(ct);

    private IOrderedQueryable<UserApplicationDal> GetAppToCheckStatusQuery(
        long chatId,
        Guid botUserId)
    {
        var lastAvailableDate = (DateTime.UtcNow - MaxApplicationAge);
        return _context
            .UserApplications
            .Where(a =>
                a.ChatId == chatId
                && a.BotUserId == botUserId)
            .Where(a =>
                (CompletedStatusesAvailableToUser.Contains(a.State)
                 && a.LastActivity > lastAvailableDate)
                || a.State == ApplicationStatus.InProgress)
            .OrderByDescending(a => a.LastActivity);
    }

}

public record UserApplicationStatus
{
    public required Guid Id { get; set; }

    public required string VacancyTitle { get; set; }
        
    public required ApplicationStatus State { get; set; }
        
    public required DateTime StartDate { get; set; }
    public required DateTime LastActivity { get; set; }
}