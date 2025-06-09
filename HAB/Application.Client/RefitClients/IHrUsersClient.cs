using Application.Client.Pages.Users;
using Refit;
using Shared.Models;
using Shared.Models.HrUsers;

namespace Application.Client.RefitClients;

public interface IHrUsersClient : IRefitClient
{
    [Post("/api/hrs/")]
    Task<IApiResponse<int>> CreateHrUser(
        [Body] CreateHrUserRequest newHr,
        CancellationToken ct);
    
    /// Update
    [Put("/api/hrs/")]
    Task<IApiResponse<int>> UpdateHrUser(
        [Body] UpdateHrUserRequest hrEdit,
        CancellationToken ct);
    
    /// Get list
    [Get("/api/hrs/")]
    Task<IApiResponse<PaginationResult<HrUserListItem>>> Get(
        [Query] Pagination pagination,
        [Query] string search,
        CancellationToken ct);
}