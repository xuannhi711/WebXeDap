using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Features.Catalog.Queries;

public static class ProductQueries
{
	public static IQueryable<Product> ByID(this IQueryable<Product> query, int id)
	{
		return query.Where(x => x.ID == id);
	}

	public static IQueryable<Product> ApplySorting(
		this IQueryable<Product> query,
		bool isAscending,
		string sortBy = "id"
	)
	{
		return (sortBy.ToLowerInvariant(), isAscending) switch
		{
			("id", true) => query.OrderBy(p => p.ID),
			("id", false) => query.OrderByDescending(p => p.ID),

			("name", true) => query.OrderBy(p => p.Name),
			("name", false) => query.OrderByDescending(p => p.Name),

			("price", true) => query.OrderBy(p => p.Price),
			("price", false) => query.OrderByDescending(p => p.Price),

			("created_at", true) => query.OrderBy(p => p.CreatedAt),
			("created_at", false) => query.OrderByDescending(p => p.CreatedAt),

			_ => isAscending ? query.OrderBy(x => x.ID) : query.OrderByDescending(x => x.ID),
		};
	}

	public static IQueryable<Product> ApplySorting(
		this IQueryable<Product> query,
		FilterProductCommand req
	)
	{
		return query.ApplySorting(req.IsAscending, req.SortBy);
	}

	public static IQueryable<Product> Filter(
		this IQueryable<Product> query,
		FilterProductCommand req
	)
	{
		if (!string.IsNullOrWhiteSpace(req.Keyword))
		{
			query = query.Where(p => p.Name.Contains(req.Keyword));
		}

		if (req.CategoryIDs is { Length: > 0 })
		{
			query = query.Where(product =>
				product.Categories.Any(category => req.CategoryIDs.Contains(category.ID))
			);
		}

		if (req.MinPrice.HasValue)
		{
			query = query.Where(p => p.Price >= req.MinPrice.Value);
		}

		if (req.MaxPrice.HasValue)
		{
			query = query.Where(p => p.Price <= req.MaxPrice.Value);
		}

		return query;
	}
}
