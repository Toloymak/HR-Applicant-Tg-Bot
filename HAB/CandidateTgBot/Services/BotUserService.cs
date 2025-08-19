using DataLayer.Contexts;
using DataLayer.Dals;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace CandidateTgBot.Services;

public class BotUserService
{
    private readonly HrBotContext _context;
    private readonly ILogger<BotUserService> _logger;

    public BotUserService(HrBotContext context, ILogger<BotUserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new bot user or updates existing user's last activity
    /// </summary>
    /// <param name="user">Telegram User object</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The BotUserDal entity (new or existing)</returns>
    public async Task<BotUserDal> CreateOrUpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        
        // Try to find existing user by Telegram ID
        var existingUser = await _context.BotUsers
            .FirstOrDefaultAsync(u => u.TgId == user.Id, cancellationToken);

        if (existingUser != null)
        {
            // Update last activity for existing user
            existingUser.LastActivity = now;
            
            // Update username if it changed
            if (existingUser.TgName != user.Username)
            {
                _logger.LogInformation(
                    "Updating username for user {TgId}: {OldUsername} -> {NewUsername}",
                    user.Id, existingUser.TgName, user.Username);
                existingUser.TgName = user.Username;
            }

            await _context.SaveChangesAsync(cancellationToken);
            
            _logger.LogDebug(
                "Updated last activity for user {TgId} ({Username})",
                user.Id, user.Username);
                
            return existingUser;
        }

        // Create new user
        var newUser = new BotUserDal
        {
            Id = Guid.NewGuid(),
            TgId = user.Id,
            TgName = user.Username,
            LastActivity = now,
            CreatedAt = now
        };

        _context.BotUsers.Add(newUser);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Created new bot user: {TgId} ({Username})",
            user.Id, user.Username);

        return newUser;
    }

    /// <summary>
    /// Updates the last activity timestamp for a user
    /// </summary>
    /// <param name="tgId">Telegram User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task UpdateLastActivityAsync(long tgId, CancellationToken cancellationToken = default)
    {
        var user = await _context.BotUsers
            .FirstOrDefaultAsync(u => u.TgId == tgId, cancellationToken);

        if (user != null)
        {
            user.LastActivity = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            
            _logger.LogDebug("Updated last activity for user {TgId}", tgId);
        }
        else
        {
            _logger.LogWarning("Attempted to update activity for non-existent user {TgId}", tgId);
        }
    }

    /// <summary>
    /// Gets a bot user by Telegram ID
    /// </summary>
    /// <param name="tgId">Telegram User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>BotUserDal or null if not found</returns>
    public async Task<BotUserDal?> GetUserByTgIdAsync(long tgId, CancellationToken cancellationToken = default)
    {
        return await _context.BotUsers
            .FirstOrDefaultAsync(u => u.TgId == tgId, cancellationToken);
    }
}
