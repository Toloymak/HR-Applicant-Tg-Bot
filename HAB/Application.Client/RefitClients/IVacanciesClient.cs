using Refit;
using Shared.Models;

namespace Application.Client.RefitClients;

public interface IVacanciesClient : IRefitClient
{
    [Get("/api/vacancies")]
    Task<IApiResponse<PaginationResult<VacancyListItem>>> GetVacancies(
        Pagination pagination,
        VacancyListFilter filter,
        CancellationToken ct);
}