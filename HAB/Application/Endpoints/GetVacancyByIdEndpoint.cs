using Application.Policies;
using Domain.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Application.Extensions;

namespace Application.Endpoints;

public class GetVacancyByIdEndpoint : IEndpointDefinition
{
    public static void Define(IEndpointRouteBuilder builder)
        => builder
            .MapGet("/api/vacancies/{id}", GetVacancyById)
            .RequireAuthorizationPolicy(Policy.Manage.Name)
            .WithDescription("Get a single vacancy by ID");

    private static async Task<Results<Ok<VacancyDetailsDto>, NotFound<string>>>
        GetVacancyById(
            Guid id,
            VacanciesRepository repository,
            CancellationToken ct = default)
    {
        var result = await repository.GetDetailsById(id, ct);
        return result.Match<Results<Ok<VacancyDetailsDto>, NotFound<string>>>(
            vacancy => TypedResults.Ok(vacancy),
            ex => TypedResults.NotFound(ex.Message)
        );
    }
} 