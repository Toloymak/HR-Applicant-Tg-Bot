using CandateTgBot.Shared.Services;
using Shared.Models;

namespace ApiCore.Services;

public class AvailableVacancyProvider : IProvideAvailablePositions
{
    private readonly VacanciesRepository _repository;

    public AvailableVacancyProvider(
        VacanciesRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<Position>> GetAvailablePositions(
        CancellationToken ct)
    {
        var activeVacancies = await _repository
            .GetList(
                new Pagination(1, 100),
                new VacancyListFilter()
                {
                    IncludeArchived = false
                }, ct);

        return activeVacancies
            .Data
            .Select(x => new Position(
                x.Title,
                x.Id))
            .ToArray();
    }
}