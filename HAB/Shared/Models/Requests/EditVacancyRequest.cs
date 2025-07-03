namespace Shared.Models.Requests;

public class EditVacancyRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DefaultRejectText { get; set; } = string.Empty;
    public string DefaultAcceptedToReviewText { get; set; } = string.Empty;
} 