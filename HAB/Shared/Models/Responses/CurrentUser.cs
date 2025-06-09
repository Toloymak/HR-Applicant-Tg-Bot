namespace Shared.Models.Responses;

public class CurrentUser
{
    public string Name { get; set; }
    public Dictionary<string, string> Claims { get; set; }
}