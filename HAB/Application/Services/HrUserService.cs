using Application.Client.Pages.Users;
using Application.Client.Services.HrUsers;
using LanguageExt;

namespace Application.Services;

public class HrUserService : IHrUserService
{
    public Task<Either<Exception, int>> CreateHrUser(
        NewHrUser newHr, CancellationToken ct)
    {
        throw new NotImplementedException("TDB");
    }

    public Task<Either<Exception, int>> UpdateHrUser(
        EditHrUser newHr, CancellationToken ct)
    {
        throw new NotImplementedException("TDB");
    }
}