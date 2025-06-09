using System.Diagnostics.CodeAnalysis;
using Application.Extensions;
using Application.Policies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace Application.Endpoints;

public class GetVacanciesListEndpoint : IEndpointDefinition
{
    public static void Define(IEndpointRouteBuilder builder)
        => builder
            .MapGet("/api/vacancies", GetVacancies)
            .RequireAuthorizationPolicy(Policy.Manage.Name)
            .WithDescription("Some test ");
    
    private static async Task<Results<Ok<PaginationResult<VacancyListItem>>, BadRequest<string>>> 
        GetVacancies(
            [AsParameters] Pagination pagination, 
            [AsParameters] VacancyListFilter filter,
            CancellationToken ct = default)
    {
        await Task.Delay(100, ct);
        
        var items = GetItems(pagination.PageNumber, pagination.PageSize);

        return TypedResults.Ok(new PaginationResult<VacancyListItem>()
        {
            Data = items,
            TotalCount = 1000,
            PageNumber = 1,
        });
    }

    private static List<VacancyListItem> GetItems(int page, int count)
    {
        var items = new List<VacancyListItem>();

        var firstItem = page * 10 + 1;
        var lastItem = firstItem + count;
        for (var i = firstItem; i < lastItem; i++)
        {
            items.Add(new VacancyListItem
            {
                Id = Guid.NewGuid(),
                HrId = Guid.CreateVersion7(),
                Title = $"Test vacancy N {i}",
                HrName = "Galina Petrovna",
                ApplicationsCount = new Random().Next(1, 375),
                CreatedAt = DateOnly.FromDateTime(DateTime.Now),
            });
        }
        
        return items;
    }
}