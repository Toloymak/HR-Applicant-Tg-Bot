using LanguageExt;
using Shared.Models.Requests;

namespace UiShared.Sercvies;

public interface IEditVacancy
{
    Task<Either<Exception, Guid>> EditVacancy(EditVacancyRequest request, CancellationToken ct);
} 