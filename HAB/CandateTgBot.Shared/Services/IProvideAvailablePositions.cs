namespace CandateTgBot.Shared.Services;

public interface IProvideAvailablePositions
{
    Task<IReadOnlyCollection<Position>> GetAvailablePositions(CancellationToken ct);
}


public record Position(
    string Name,
    Guid VacancyId);