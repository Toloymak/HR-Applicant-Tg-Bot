using Application.Extensions;
using Application.Policies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Application.Endpoints;

public class TestEndpoints : IEndpointDefinition
{
    public static void Define(IEndpointRouteBuilder builder) => builder
        .MapPost("test", RandomTest)
        .RequireAuthorizationPolicy(Policy.Manage.Name)
        .WithDescription("Some test ");

    private static async Task<Results<Ok<string>, BadRequest<string>>> RandomTest(
        CancellationToken ct = default)
    {
        return TypedResults.Ok("Pong");
    }
}