namespace WebXeDap.Application.DTOs;

public record PaginatedResult<T>(IReadOnlyList<T> Items, int Total, int Page, int Size);
