namespace Shared.Models;

public class PaginationResult<T>
{
    public required IReadOnlyCollection<T> Data { get; set; }
    public required int TotalCount { get; set; }
    public required int PageNumber { get; set; }
}