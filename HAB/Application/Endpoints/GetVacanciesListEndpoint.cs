using System.Diagnostics.CodeAnalysis;
using Application.Extensions;
using Application.Policies;
using Domain.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace Application.Endpoints;

public class GetVacanciesListEndpoint : IEndpointDefinition
{
    public static void Define(IEndpointRouteBuilder builder)
        => builder
            .MapGet("/api/vacancies", GetVacancies)
            .RequireAuthorizationPolicy(Policy.Manage.Name)
            .WithDescription("Some test ");
    
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