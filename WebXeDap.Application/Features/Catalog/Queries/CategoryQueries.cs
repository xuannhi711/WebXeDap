using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Features.Catalog.Queries;

public static class CategoryQueries
{
	public static IQueryable<Category> ByName(this IQueryable<Category> query, string name)
	{
		return query.Where(x => x.Name == name);
	}

	public static IQueryable<Category> ByID(this IQueryable<Category> query, int id)
	{
		return query.Where(x => x.ID == id);
	}
}
