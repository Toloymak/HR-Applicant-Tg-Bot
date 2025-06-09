using DataLayer.Contexts;
using DataLayer.Dals;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Models;

namespace Domain.Services;

public class UserService
{
    private readonly HrBotContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(
        HrBotContext context,
        ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Guid> CreateUser(NewBotUser user, CancellationToken ct)
    {
        var existingUser = await _context.BotUsers
            .FirstOrDefaultAsync(u => u.TgId == user.TgId, cancellationToken: ct);

        if (existingUser != null)
        {
            await UpdateExisingUser(user, ct, existingUser);
            return existingUser.Id;
        }
        
        var newUser = new BotUserDal
        {
            TgId = user.TgId,
            TgName = user.TgName,
            CustomName = user.TgName,
        };

        _context.Add(newUser);
        await _context.SaveChangesAsync(ct);
        _logger.LogInformation(
            "Created new user with ID: {UserId}/{UserName}",
            newUser.TgId,
            newUser.TgName);

        return newUser.Id;
    }

    private async Task UpdateExisingUser(NewBotUser user, CancellationToken ct, BotUserDal existingUser)
    {
        existingUser.TgName = user.TgName;
        await _context.SaveChangesAsync(ct);
    }
}