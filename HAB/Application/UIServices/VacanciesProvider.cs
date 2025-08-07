using ApiCore.Services;
using LanguageExt;
using Shared.Models;
using UiShared.Sercvies;
using Shared.Models.Vacancies;

namespace Application.UIServices;

public class VacanciesProvider : IVacanciesProvider, ICreateVacancy, IEditVacancy
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

    public async Task<Either<Exception, Guid>> CreateVacancy(
        CreateVacancyRequest request, CancellationToken ct)
    {
        return await _repository.Create(request, ct);
    }

    public async Task<Either<Exception, Guid>> EditVacancy(EditVacancyRequest request, CancellationToken ct)
    {
        return await _repository.Edit(request, ct);
    }

    public async Task<Either<Exception, VacancyDetailsDto>> GetVacancyById(Guid id, CancellationToken ct)
    {
        return await _repository.GetDetailsById(id, ct);
    }
}