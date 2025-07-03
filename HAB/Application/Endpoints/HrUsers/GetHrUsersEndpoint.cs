using Application.Extensions;
using Application.Policies;
using Domain.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.Models;
using Shared.Models.HrUsers;

namespace Application.Endpoints.HrUsers;

public class GetHrUsersEndpoint : IEndpointDefinition
{
    public static void Define(IEndpointRouteBuilder builder)
    => builder
        .MapGet("/api/hrs", GetUsers)
        .RequireAuthorizationPolicy(Policy.Manage.Name)
        .WithDescription("Get HR users list with pagination");

    private static async Task<Results<Ok<PaginationResult<HrUserListItemDal>>, BadRequest<string>>>
        GetUsers(
            [AsParameters] Pagination pagination,
            HrUserRepository repository,
            string search = "",
            CancellationToken ct = default)
    {
        var data = await repository.GetList(pagination, search, ct);
        return TypedResults.Ok(data);
    }
}