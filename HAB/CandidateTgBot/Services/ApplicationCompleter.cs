using DataLayer.Contexts;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace CandidateTgBot.Services;

public class ApplicationCompleter
{
    private readonly HrBotContext _context;

    public ApplicationCompleter(
        HrBotContext context)
    {
        _context = context;
    }

    public async Task TryCompleteApplication(Guid botUserId, CancellationToken ct)
    {
        var questionsToBeCompeted = await _context.UserApplications
            .Include(x => x.Answers)
            .Include(x => x.Vacancy)
                .ThenInclude(v => v!.Questions)
            .Where(x => x.BotUserId == botUserId 
                        && x.State == ApplicationStatus.InProgress
                        && x.Answers!.Count == x.Vacancy!.Questions!.Count)
            .ToArrayAsync(ct);

        foreach (var application in questionsToBeCompeted)
            application.State = ApplicationStatus.ReviewByHr;

        await _context.SaveChangesAsync(ct);
    }
}