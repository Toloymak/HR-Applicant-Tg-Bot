using Application.Client.Services.HrUsers;
using LanguageExt;
using Shared.Models;
using Shared.Models.HrUsers;

namespace Application.Services;

public class HrUserProvider : IHrUserProvider
{
    public Task<Either<Exception, PaginationResult<HrUserListItemDal>>> Get(
        Pagination pagination,
        string search,
        CancellationToken ct)
    {
        return Task.FromResult<Either<Exception, PaginationResult<HrUserListItemDal>>>(
            new PaginationResult<HrUserListItemDal>
            {
                Data = [],
                TotalCount = 0,
                PageNumber = 1,
            });
    }
}