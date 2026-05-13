namespace WebXeDap.Application.Extensions;

public static class QueryableExtensions
{
	public static IQueryable<T> Page<T>(this IQueryable<T> query, int page, int size)
	{
		return query.Skip((page - 1) * size).Take(size);
	}
}
