using ApiCore.Services;
using Application.Extensions;
using Application.Policies;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.Models;

namespace Application.Endpoints.Vacancies;

public class GetVacanciesEndpoint : IEndpointDefinition
{
    public static void Define(IEndpointRouteBuilder builder)
        => builder
            .MapGet("/api/vacancies", GetVacancies)
            .RequireAuthorizationPolicy(Policy.Manage.Name)
            .WithDescription("Get vacancies list with pagination");

    private static async Task<Results<Ok<PaginationResult<VacancyListItem>>, BadRequest<string>>>
        GetVacancies(
            [AsParameters] Pagination pagination,
            [AsParameters] VacancyListFilter filter,
            VacanciesRepository repository,
            CancellationToken ct = default)
    {
        var result = await repository.GetList(pagination, filter, ct);
        return TypedResults.Ok(result);
    }
}