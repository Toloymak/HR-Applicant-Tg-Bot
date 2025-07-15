namespace Shared.Models.Vacancies;

public class CreateVacancyRequest
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string DefaultRejectText { get; set; }
    public required string DefaultAcceptedToReviewText { get; set; }
    public required IReadOnlyCollection<VacancyQuestionDto> Questions { get; set; }
}