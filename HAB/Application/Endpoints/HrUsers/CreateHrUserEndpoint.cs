using ApiCore.Services;
using Application.Extensions;
using Application.Policies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.HrUsers;

namespace Application.Endpoints.HrUsers;

public class CreateHrUserEndpoint : IEndpointDefinition
{
    public static void Define(
        IEndpointRouteBuilder builder)
    => builder
        .MapPost("/api/hrs", CreateUser)
        .RequireAuthorizationPolicy(Policy.Manage.Name)
        .WithDescription("Get HR users list with pagination");

    private static async Task<Results<Ok<int>, BadRequest<string>>>
        CreateUser(
            [FromBody] CreateHrUserRequest request,
            HrUserRepository repository,
            CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Alias) || string.IsNullOrWhiteSpace(request.TgName))
            return TypedResults.BadRequest("Alias and Telegram name are required.");

        var id = await repository.CreateHrUser(request, ct);

        return TypedResults.Ok(id);
    }
}