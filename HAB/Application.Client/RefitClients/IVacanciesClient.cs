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

    [Put("/api/vacancies/{id}")]
    Task<IApiResponse<Guid>> EditVacancy(
        Guid id,
        [Body] EditVacancyRequest request,
        CancellationToken ct);

    [Get("/api/vacancies/{id}")]
    Task<IApiResponse<VacancyDetailsDto>> GetVacancyById(
        Guid id,
        CancellationToken ct);
}