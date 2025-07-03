using LanguageExt;
using Shared.Models.Requests;

namespace UiShared.Sercvies;

public interface ICreateVacancy
{
    Task<Either<Exception, Guid>> CreateVacancy(
        CreateVacancyRequest request,
        CancellationToken ct);
}