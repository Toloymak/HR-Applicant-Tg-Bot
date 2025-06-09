namespace Application.Client.Pages.Users;

public record EditHrUser
{
    public required int Id { get; set; }
    public required string TgName { get; set; } = string.Empty;
    public required string Alias { get; set; } = string.Empty;
    public required string Position { get; set; } = string.Empty;
}