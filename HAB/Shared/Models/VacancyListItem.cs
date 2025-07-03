namespace Shared.Models;

public class VacancyListItem
{
    public required Guid Id { get; set; }
    
    public required string Title { get; set; } = string.Empty;
    
    public required int? HrId { get; set; }
    public required string? HrName { get; set; } = string.Empty;
    
    public required int ApplicationsCount { get; set; }
    
    public required DateTime CreatedAt { get; set; }
}