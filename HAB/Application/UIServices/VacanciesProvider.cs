using LanguageExt;
using Shared.Models;
using UiShared.Sercvies;
using Domain.Services;

namespace Application.UIServices;

public class VacanciesProvider : IVacanciesProvider
{
    private readonly VacanciesRepository _repository;

    public VacanciesProvider(VacanciesRepository repository)
    {
        _repository = repository;
    }

    public async Task<Either<Exception, PaginationResult<VacancyListItem>>> GetVacancies(
        Pagination pagination,
        VacancyListFilter filter,
        CancellationToken ct)
    {
        try
        {
            var result = await _repository.GetList(pagination, filter, ct);
            return result;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}