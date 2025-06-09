using Application.Extensions;
using Application.Policies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.HrUsers;

namespace Application.Endpoints.HrUsers;

public class UpdateHrUserEndpoint : IEndpointDefinition
{
    public static void Define(
        IEndpointRouteBuilder builder)
    => builder
        .MapPut("/api/hrs", UpdateUser)
        .RequireAuthorizationPolicy(Policy.Manage.Name)
        .WithDescription("Get HR users list with pagination");

    private static async Task<Results<Ok<int>, BadRequest<string>>>
        UpdateUser(
            [FromBody] CreateHrUserRequest request,
            CancellationToken ct = default)
    {
        await Task.Delay(200, ct); // Simulate async operation
        
        if (string.IsNullOrWhiteSpace(request.Alias) || string.IsNullOrWhiteSpace(request.TgName))
            return TypedResults.BadRequest("Alias and Telegram name are required.");

        return TypedResults.Ok(1);
    }
}