using Application.Client.Extensions;
using Application.Client.RefitClients;
using LanguageExt;
using Shared.Models;
using UiShared.Sercvies;
using Shared.Models.Vacancies;

namespace Application.Client.Services;

internal class VacanciesProvider : IVacanciesProvider, ICreateVacancy, IEditVacancy
{
    private readonly IVacanciesClient _vacanciesClient;

    public VacanciesProvider(IVacanciesClient vacanciesClient)
    {
        _vacanciesClient = vacanciesClient;
    }

    public async Task<Either<Exception, PaginationResult<VacancyListItem>>> GetVacancies(
        Pagination pagination, VacancyListFilter filter, CancellationToken ct)
    {
        return await _vacanciesClient
            .GetVacancies(pagination, filter, ct)
            .ToEither();
    }

    public async Task<Either<Exception, Guid>> CreateVacancy(
        CreateVacancyRequest request, CancellationToken ct)
    {
        try
        {
            var response = await _vacanciesClient.CreateVacancy(request, ct);
            if (response.IsSuccessStatusCode && response.Content != null)
                return response.Content;
            return new Exception(response.Error?.Message ?? "Unknown error");
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Either<Exception, Guid>> EditVacancy(EditVacancyRequest request, CancellationToken ct)
    {
        try
        {
            var response = await _vacanciesClient.EditVacancy(request.Id, request, ct);
            if (response.IsSuccessStatusCode && response.Content != null)
                return response.Content;
            return new Exception(response.Error?.Message ?? "Unknown error");
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Either<Exception, VacancyDetailsDto>> GetVacancyById(Guid id, CancellationToken ct)
    {
        try
        {
            var response = await _vacanciesClient.GetVacancyById(id, ct);
            if (response.IsSuccessStatusCode && response.Content != null)
                return response.Content;
            return new Exception(response.Error?.Message ?? "Unknown error");
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}