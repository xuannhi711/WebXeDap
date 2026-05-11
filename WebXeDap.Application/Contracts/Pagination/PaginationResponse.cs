namespace WebXeDap.Application.Contracts.Pagination;

public record PaginatedResult<T>(IReadOnlyList<T> Items, int Total, int Page, int Size);
