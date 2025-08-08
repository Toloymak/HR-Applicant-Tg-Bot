namespace CandidateTgBot.Services;

public interface IProvideAvailablePositions
{
    Task<IReadOnlyCollection<Position>> GetAvailablePositions(CancellationToken ct);
}


public record Position(
    string Name,
    Guid VacancyId);

public class ProvideAvailablePositionsMock : IProvideAvailablePositions
{
    public Task<IReadOnlyCollection<Position>> GetAvailablePositions(CancellationToken ct)
    {
        var positions = new List<Position>
        {
            new Position("Software Engineer", Guid.NewGuid()),
            new Position("Data Scientist", Guid.NewGuid()),
            new Position("Product Manager", Guid.NewGuid())
        };

        return Task.FromResult<IReadOnlyCollection<Position>>(positions);
    }
}
