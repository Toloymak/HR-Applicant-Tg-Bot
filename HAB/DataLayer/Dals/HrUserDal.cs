namespace DataLayer.Dals;

public class HrUserDal
{
    public int Id { get; set; }
    
    public required Guid BotUserId { get; set; }
    public BotUserDal BotUser { get; set; } = null!;
    
    public required string Alias { get; set; } = string.Empty;
    public required string Position { get; set; } = string.Empty;
}