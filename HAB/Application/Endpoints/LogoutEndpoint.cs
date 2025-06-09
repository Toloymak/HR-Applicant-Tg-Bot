namespace Application.Endpoints;

public class LogoutEndpoint : IEndpointDefinition
{
    public static void Define(IEndpointRouteBuilder builder)
        => builder.MapGet("/api/auth/logout", (HttpContext context) =>
        {
            context.Response.Cookies.Delete("auth");
            return Results.Ok();
        });
}