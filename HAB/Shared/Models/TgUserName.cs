namespace Shared.Models;

public record TgUserName(string Name)
{
    public override string ToString()
        => $"@{Name}";
    
    public string ToTgLink()
        => $"https://t.me/{Name}";
    
    public string TgLinkWithAt()
        => $"@{Name}";
}