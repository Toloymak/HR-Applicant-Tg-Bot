using LanguageExt;
using Shared.Models.Requests;

namespace UiShared.Sercvies;

public interface ICreateVacation
{
    Task<Either<Exception, Guid>> CreateVacation(
        CreateVacationRequest request,
        CancellationToken ct);
}