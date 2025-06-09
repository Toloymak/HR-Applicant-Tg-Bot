using Application.Client.Extensions;
using Application.Client.Pages.Users;
using Application.Client.RefitClients;
using LanguageExt;
using Shared.Models.HrUsers;

namespace Application.Client.Services.HrUsers;

public interface IHrUserService
{
    Task<Either<Exception, int>> CreateHrUser(
        NewHrUser newHr, CancellationToken ct);

    Task<Either<Exception, int>> UpdateHrUser(
        EditHrUser newHr, CancellationToken ct);
}

internal class HrUserService : IHrUserService
{
    private readonly IHrUsersClient _hrUsersClient;

    public HrUserService(IHrUsersClient hrUsersClient)
    {
        _hrUsersClient = hrUsersClient;
    }

    public async Task<Either<Exception, int>> CreateHrUser(NewHrUser newHr, CancellationToken ct)
    {
        var request = new CreateHrUserRequest
        {
            TgName = newHr.TgName,
            Alias = newHr.Alias,
            Position = newHr.Position
        };
        return await _hrUsersClient
            .CreateHrUser(request, ct)
            .ToEither();
    }
    
    public async Task<Either<Exception, int>> UpdateHrUser(EditHrUser newHr, CancellationToken ct)
    {
        var request = new UpdateHrUserRequest()
        {
            Id = newHr.Id,
            TgName = newHr.TgName,
            Alias = newHr.Alias,
            Position = newHr.Position
        };
        return await _hrUsersClient
            .UpdateHrUser(request, ct)
            .ToEither();
    }
}