using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Features.Catalog.Queries;

public static class ProductQueries
{
	public static IQueryable<Product> ByName(this IQueryable<Product> query, string name)
	{
		return query.Where(x => x.Name == name);
	}

	public static IQueryable<Product> ByID(this IQueryable<Product> query, int id)
	{
		return query.Where(x => x.ID == id);
	}
}
