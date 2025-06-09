namespace Application.Client.Pages.Vacancy;

public class NewVacancy
{
    public string Name { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    
    public string DefaultRejectText { get; set; } = String.Empty;
    public string DefaultAcceptedToReviewText { get; set; } = String.Empty;
}