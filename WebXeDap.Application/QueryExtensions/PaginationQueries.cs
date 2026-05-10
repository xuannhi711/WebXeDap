using WebXeDap.Application.DTOs;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.QueryExtensions;

public static class PaginationQueries
{
	public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int page, int size)
	{
		return query.Skip((page - 1) * size).Take(size);
	}
}
