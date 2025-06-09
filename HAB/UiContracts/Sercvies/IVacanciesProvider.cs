using LanguageExt;
using Shared.Models;

namespace UiShared.Sercvies;

public interface IVacanciesProvider
{
    Task<Either<Exception, PaginationResult<VacancyListItem>>> GetVacancies(
        Pagination pagination,
        VacancyListFilter filter,
        CancellationToken ct);
}