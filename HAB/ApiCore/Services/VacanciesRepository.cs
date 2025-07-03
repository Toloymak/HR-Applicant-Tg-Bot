using DataLayer.Contexts;
using DataLayer.Dals;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

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
} 