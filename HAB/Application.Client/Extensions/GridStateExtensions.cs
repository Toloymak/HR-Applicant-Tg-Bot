using MudBlazor;
using Shared.Models;

namespace Application.Client.Extensions;

public static class GridStateExtensions
{
    public static Pagination ToPagination<T>(this GridState<T> state)
        => new(
            PageNumber: state.Page,
            PageSize: state.PageSize);
}