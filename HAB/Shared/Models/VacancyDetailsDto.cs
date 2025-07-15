using Shared.Models.Vacancies;

namespace Shared.Models;

public record VacancyDetailsDto
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string HrName { get; init; }
    public required int ApplicationsCount { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string DefaultRejectText { get; init; }
    public required string DefaultAcceptedToReviewText { get; init; }
    public required IReadOnlyCollection<VacancyQuestionDto> Questions { get; init; }
    public required bool IsActive { get; init; }
} 