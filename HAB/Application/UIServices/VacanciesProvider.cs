using LanguageExt;
using Shared.Models;
using UiShared.Sercvies;

namespace Application.UIServices;

public class VacanciesProvider : IVacanciesProvider
{
    public async Task<Either<Exception, PaginationResult<VacancyListItem>>> GetVacancies(
        Pagination pagination,
        VacancyListFilter filter,
        CancellationToken ct)
    {
        await Task.CompletedTask;
        return new PaginationResult<VacancyListItem>
        {
            Data = [],
            TotalCount = 0,
            PageNumber = 1,
        };
    }
}