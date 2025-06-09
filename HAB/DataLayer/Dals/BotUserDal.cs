using System.ComponentModel.DataAnnotations;

namespace DataLayer.Dals;

public class BotUserDal
{
    public Guid Id { get; set; }

    // TODO: Check with docs
    [MaxLength(200)]
    public required string TgId { get; set; }
    
    [MaxLength(200)]
    public required string TgName { get; set; }
    
    [MaxLength(200)]
    public required string CustomName { get; set; }
}