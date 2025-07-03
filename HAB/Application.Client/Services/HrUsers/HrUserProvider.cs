using Application.Client.Extensions;
using Application.Client.Pages.Users;
using Application.Client.RefitClients;
using LanguageExt;
using Shared.Models;
using Shared.Models.HrUsers;

namespace Application.Client.Services.HrUsers;

public interface IHrUserProvider
{
    Task<Either<Exception, PaginationResult<HrUserListItemDal>>> Get(
        Pagination pagination, string search, CancellationToken ct);
}

internal class HrUserProvider : IHrUserProvider
{
    private readonly IHrUsersClient _hrUsersClient;

    public HrUserProvider(IHrUsersClient hrUsersClient)
    {
        _hrUsersClient = hrUsersClient;
    }

    public async Task<Either<Exception, PaginationResult<HrUserListItemDal>>> Get(
        Pagination pagination, string search, CancellationToken ct)
    {
        return await _hrUsersClient.Get(pagination, search, ct)
            .ToEither();
    }
}