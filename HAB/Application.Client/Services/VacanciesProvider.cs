using Application.Client.Extensions;
using Application.Client.RefitClients;
using LanguageExt;
using Shared.Models;
using UiShared.Sercvies;

namespace Application.Client.Services;

internal class VacanciesProvider : IVacanciesProvider
{
    private readonly IVacanciesClient _vacanciesClient;

    public VacanciesProvider(IVacanciesClient vacanciesClient)
    {
        _vacanciesClient = vacanciesClient;
    }

    public async Task<Either<Exception, PaginationResult<VacancyListItem>>> GetVacancies(
        Pagination pagination, VacancyListFilter filter, CancellationToken ct)
    {
        return await _vacanciesClient
            .GetVacancies(pagination, filter, ct)
            .ToEither();
    }
}