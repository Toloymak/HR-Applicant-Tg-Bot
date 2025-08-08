namespace CandateTgBot.Shared.Services;

public interface IGetVacancyInfo
{
    Task<VacancyInfo?> Get(Guid vacancyId, CancellationToken ct);
}

public record VacancyInfo(
    Guid VacancyId,
    string Title,
    string Description);

