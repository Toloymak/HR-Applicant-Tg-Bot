using Refit;
using Shared.Models;
using Shared.Models.Requests;

namespace Application.Client.RefitClients;

public interface IVacanciesClient : IRefitClient
{
    [Get("/api/vacancies")]
    Task<IApiResponse<PaginationResult<VacancyListItem>>> GetVacancies(
        Pagination pagination,
        VacancyListFilter filter,
        CancellationToken ct);

    [Post("/api/vacancies")]
    Task<IApiResponse<Guid>> CreateVacancy(
        [Body] CreateVacancyRequest request,
        CancellationToken ct);
}