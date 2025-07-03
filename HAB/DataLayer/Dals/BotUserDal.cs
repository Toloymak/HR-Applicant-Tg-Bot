using System.ComponentModel.DataAnnotations;

namespace DataLayer.Dals;

public class BotUserDal
{
    public required Guid Id { get; set; }

    // TODO: Check with docs
    [MaxLength(200)]
    public required string? TgId { get; set; }
    
    [MaxLength(200)]
    public required string TgName { get; set; }
}