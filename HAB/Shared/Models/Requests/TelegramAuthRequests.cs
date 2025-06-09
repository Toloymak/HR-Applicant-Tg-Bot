namespace Shared.Models.Requests;

public record TelegramAuthRequests
{
    public long Id { get; set; }
    public string First_name { get; set; }
    public string Last_name { get; set; }
    public string Username { get; set; }
    public string Photo_url { get; set; }
    public int Auth_date { get; set; }
    public string Hash { get; set; }
}