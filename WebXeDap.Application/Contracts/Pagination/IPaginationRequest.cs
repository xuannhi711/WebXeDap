namespace WebXeDap.Application.Contracts.Pagination;

public interface IPaginatedRequest
{
	int Page { get; }
	int Size { get; }
}

public static class PaginationQueryableExtensions
{
	public static IQueryable<T> Paginate<T>(this IQueryable<T> query, IPaginatedRequest req)
	{
		return query.Skip((req.Page - 1) * req.Size).Take(req.Size);
	}
}
