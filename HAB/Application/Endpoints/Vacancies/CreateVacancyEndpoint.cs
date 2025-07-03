using Application.Extensions;
using Application.Policies;
using Domain.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.Requests;

namespace Application.Endpoints.Vacancies;

public class CreateVacancyEndpoint : IEndpointDefinition
{
    public static void Define(IEndpointRouteBuilder builder)
        => builder
            .MapPost("/api/vacancies", CreateVacancy)
            .RequireAuthorizationPolicy(Policy.Manage.Name)
            .WithDescription("Create a new vacancy");

    private static async Task<Results<Ok<Guid>, BadRequest<string>>>
        CreateVacancy(
            [FromBody] CreateVacancyRequest request,
            VacanciesRepository repository,
            CancellationToken ct = default)
    {
        var result = await repository.Create(request, ct);
        return result.Match<Results<Ok<Guid>, BadRequest<string>>>(
            id => TypedResults.Ok(id),
            ex => TypedResults.BadRequest(ex.Message)
        );
    }
} 