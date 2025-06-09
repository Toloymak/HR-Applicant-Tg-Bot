using Application.Client.Pages.Users;
using Application.Client.Services.HrUsers;
using LanguageExt;
using Shared.Models;

namespace Application.Services;

public class HrUserProvider : IHrUserProvider
{
    public Task<Either<Exception, PaginationResult<HrUserListItem>>> Get(
        Pagination pagination,
        string search,
        CancellationToken ct)
    {
        return Task.FromResult<Either<Exception, PaginationResult<HrUserListItem>>>(
            new PaginationResult<HrUserListItem>
            {
                Data = [],
                TotalCount = 0,
                PageNumber = 1,
            });
    }
}