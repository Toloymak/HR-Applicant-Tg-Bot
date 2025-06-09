using Shared.Models.Responses;

namespace Application.Endpoints;

public class WhoAmIEndpoint : IEndpointDefinition
{
    public static void Define(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/api/whoAmI", WhoAmI)
            .WithDescription("Returns information about the current user.")
            .RequireAuthorization();
    }

    private static IResult WhoAmI(HttpContext context)
    {
        var user = context.User;

        if (user?.Identity?.IsAuthenticated != true)
            return Results.Unauthorized();

        var name = user.Identity.Name ?? "";
        var claims = user.Claims.ToDictionary(c => c.Type, c => c.Value);

        return Results.Ok(new CurrentUser
        {
            Name = name,
            Claims = claims
        });
    }
}