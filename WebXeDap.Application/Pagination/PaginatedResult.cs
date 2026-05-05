namespace WebXeDap.Application.Pagination;

public record PaginatedResult<T>(
	IReadOnlyList<T> Items,
	int TotalCount,
	int PageNumber,
	int PageSize
);
