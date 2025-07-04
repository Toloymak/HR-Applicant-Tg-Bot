using DataLayer.Contexts;
using DataLayer.Dals;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using LanguageExt;
using Shared.Models.Vacancies;

namespace Domain.Services;

public class VacanciesRepository
{
    private readonly HrBotContext _context;

    public VacanciesRepository(HrBotContext context)
    {
        _context = context;
    }

    public async Task<PaginationResult<VacancyListItem>> GetList(
        Pagination pagination,
        VacancyListFilter filter,
        CancellationToken ct)
    {
        var pageNumber = pagination.PageNumber < 1 ? 1 : pagination.PageNumber;
        var pageSize = pagination.PageSize < 1 ? 10 : pagination.PageSize;
        var skip = (pageNumber - 1) * pageSize;

        var query = _context.Vacancies
            .AsNoTracking()
            .Where(x => filter.IncludeArchived || !x.IsArchived) 
            .Select(x => new VacancyListItem
            {
                Id = x.Id,
                Title = x.Title,
                HrId = x.HrId,
                HrName = x.Hr != null ? x.Hr.Alias : null,
                ApplicationsCount = x.Applications!.Count,
                CreatedAt = x.CreatedAt
            });

        var totalCount = await query.CountAsync(ct);
        var data = await query
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginationResult<VacancyListItem>
        {
            Data = data,
            TotalCount = totalCount,
            PageNumber = pagination.PageNumber
        };
    }

    public async Task<Either<Exception, Guid>> Create(
        CreateVacancyRequest request, CancellationToken ct)
    {
        try
        {
            var vacancy = new VacancyDal
            {
                Id = Guid.CreateVersion7(),
                Title = request.Name,
                Description = request.Description,
                DefaultRejectText = request.DefaultRejectText,
                FinishedApplicationText = request.DefaultAcceptedToReviewText,
                CreatedAt = DateTime.UtcNow,
            };
            _context.Vacancies.Add(vacancy);
            await _context.SaveChangesAsync(ct);
            return vacancy.Id;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Either<Exception, Guid>> Edit(EditVacancyRequest request, CancellationToken ct)
    {
        try
        {
            var vacancy = await _context.Vacancies.FindAsync(new object[] { request.Id }, ct);
            if (vacancy == null)
                return new Exception("Vacancy not found");
            vacancy.Title = request.Name;
            vacancy.Description = request.Description;
            vacancy.DefaultRejectText = request.DefaultRejectText;
            vacancy.FinishedApplicationText = request.DefaultAcceptedToReviewText;
            await _context.SaveChangesAsync(ct);
            return vacancy.Id;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Either<Exception, VacancyListItem>> GetById(Guid id, CancellationToken ct)
    {
        try
        {
            var vacancy = await _context.Vacancies
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new VacancyListItem
                {
                    Id = x.Id,
                    Title = x.Title,
                    HrId = x.HrId,
                    HrName = x.Hr != null ? x.Hr.Alias : null,
                    ApplicationsCount = x.Applications != null ? x.Applications.Count : 0,
                    CreatedAt = x.CreatedAt
                })
                .FirstOrDefaultAsync(ct);
            if (vacancy == null)
                return new Exception("Vacancy not found");
            return vacancy;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Either<Exception, VacancyDetailsDto>> GetDetailsById(Guid id, CancellationToken ct)
    {
        try
        {
            var vacancy = await _context.Vacancies
                .Where(x => x.Id == id)
                .Select(x => new VacancyDetailsDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    HrName = x.Hr != null ? x.Hr.Alias : string.Empty,
                    ApplicationsCount = x.Applications != null ? x.Applications.Count : 0,
                    CreatedAt = x.CreatedAt,
                    DefaultRejectText = x.DefaultRejectText,
                    DefaultAcceptedToReviewText = x.FinishedApplicationText
                })
                .FirstOrDefaultAsync(ct);
            if (vacancy == null)
                return new Exception("Vacancy not found");
            return vacancy;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
} 