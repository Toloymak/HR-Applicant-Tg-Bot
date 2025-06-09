using Application.Client.Pages.Users;
using Application.Extensions;
using Application.Policies;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.Models;

namespace Application.Endpoints.HrUsers;

public class GetHrUsersEndpoint : IEndpointDefinition
{
    public static void Define(IEndpointRouteBuilder builder)
    => builder
        .MapGet("/api/hrs", GetUsers)
        .RequireAuthorizationPolicy(Policy.Manage.Name)
        .WithDescription("Get HR users list with pagination");

    private static async Task<Results<Ok<PaginationResult<HrUserListItem>>, BadRequest<string>>>
        GetUsers(
            [AsParameters] Pagination pagination,
            string search,
            CancellationToken ct = default)
    {
        var data = new PaginationResult<HrUserListItem>
        {
            Data =
            [
                new HrUserListItem
                {

                    Id = 1,
                    Alias = "Liza",
                    TgName = "@liza_hr_top_mega_hunter",
                    Position = "BOSS"
                },
                new HrUserListItem
                {
                    Id = 2,
                    Alias = "Vova",
                    TgName = "@vova_prosto_krasava",
                    Position = "Mascot"
                },
                new HrUserListItem
                {
                    Id = 3,
                    Alias = "Jennifer",
                    TgName = "@jenny42",
                    Position = "Hunter"
                },
                new HrUserListItem
                {
                    Id = 4,
                    Alias = "John",
                    TgName = "@john_doe",
                    Position = "Sourcer"
                },
            ],
            TotalCount = 15,
            PageNumber = pagination.PageNumber
        };
        return TypedResults.Ok(data);
    }
}