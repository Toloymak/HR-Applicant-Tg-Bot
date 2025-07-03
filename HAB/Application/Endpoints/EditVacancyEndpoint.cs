using Application.Extensions;
using Application.Policies;
using Domain.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.Requests;

namespace Application.Endpoints;

public class EditVacancyEndpoint : IEndpointDefinition
{
    public static void Define(IEndpointRouteBuilder builder)
        => builder
            .MapPut("/api/vacancies/{id}", EditVacancy)
            .RequireAuthorizationPolicy(Policy.Manage.Name)
            .WithDescription("Edit a vacancy");

    private static async Task<Results<Ok<Guid>, BadRequest<string>>>
        EditVacancy(
            Guid id,
            [FromBody] EditVacancyRequest request,
            VacanciesRepository repository,
            CancellationToken ct = default)
    {
        request.Id = id;
        var result = await repository.Edit(request, ct);
        return result.Match<Results<Ok<Guid>, BadRequest<string>>>(
            vid => TypedResults.Ok(vid),
            ex => TypedResults.BadRequest(ex.Message)
        );
    }
} 