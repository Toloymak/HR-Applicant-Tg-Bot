namespace Shared.Models.Requests;

public class CreateVacationRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public string DefaultRejectText { get; set; } = string.Empty;
    public string DefaultAcceptedToReviewText { get; set; } = string.Empty;
}