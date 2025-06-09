namespace Shared.Models;

public record VacancyListFilter
{
    public bool IncludeArchived { get; set; } = false;
}