using System.ComponentModel.DataAnnotations;

namespace DataLayer.Dals;

public class BotUserDal
{
    public required Guid Id { get; set; }

    /// <summary>
    /// Telegram User ID - up to 52 bits (stored as long)
    /// Null for HR users created before they interact with the bot
    /// </summary>
    public long? TgId { get; set; }
    
    /// <summary>
    /// Telegram Username - 5 to 32 characters (a-z, 0-9, underscores)
    /// Can be null if user has no username
    /// </summary>
    [MaxLength(32)]
    public string? TgName { get; set; }
    
    /// <summary>
    /// Date and time of the user's last activity
    /// </summary>
    public required DateTime LastActivity { get; set; }
    
    /// <summary>
    /// Date and time when the user was first created
    /// </summary>
    public required DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Navigation property for user applications submitted by this bot user
    /// </summary>
    public ICollection<UserApplicationDal> Applications { get; set; } = new List<UserApplicationDal>();
}