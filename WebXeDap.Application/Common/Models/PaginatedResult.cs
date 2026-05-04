namespace WebXeDap.Application.Common.Models;

public sealed class PaginatedResult<T>(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
{
	public IReadOnlyList<T> Items { get; } = items;
	public int TotalCount { get; } = totalCount;
	public int PageNumber { get; } = pageNumber;
	public int PageSize { get; } = pageSize;
}
