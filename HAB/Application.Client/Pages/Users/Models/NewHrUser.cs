namespace Application.Client.Pages.Users;

public record NewHrUser
{
    public string TgName { get; set; } = string.Empty;
    public string Alias { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
}