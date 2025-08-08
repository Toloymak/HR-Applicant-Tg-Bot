using CandateTgBot.Shared.Services;

namespace ApiCore.Services;

public class GetVacancyInfoMock : IGetVacancyInfo
{
    public Task<VacancyInfo?> Get(Guid vacancyId, CancellationToken ct)
    {
        var info = new VacancyInfo(
            VacancyId: vacancyId,
            Title: $"Vacancy {vacancyId.ToString()[..8]}",
            Description: "This is a mock description for the selected vacancy. It outlines responsibilities, requirements, and benefits."
        );
        return Task.FromResult<VacancyInfo?>(info);
    }
}

