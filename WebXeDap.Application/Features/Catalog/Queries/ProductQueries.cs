using WebXeDap.Application.DTOs;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Features.Catalog.Queries;

public static class ProductQueries
{
	public static IQueryable<Product> ByName(this IQueryable<Product> query, string name)
	{
		return query.Where(x => x.Name.Contains(name));
	}

	public static IQueryable<Product> ByID(this IQueryable<Product> query, int id)
	{
		return query.Where(x => x.ID == id);
	}

	public static IQueryable<Product> ByCategoryID(this IQueryable<Product> query, int categoryID)
	{
		return query.Where(x => x.Categories.Any(c => c.ID == categoryID));
	}

	public static IQueryable<Product> ByCategoryIDs(
		this IQueryable<Product> query,
		int[] categoryIDs
	)
	{
		return query.Where(x => x.Categories.Any(c => categoryIDs.Contains(c.ID)));
	}

	public static IQueryable<Product> ByMinPrice(this IQueryable<Product> query, decimal minPrice)
	{
		return query.Where(x => x.Price >= minPrice);
	}

	public static IQueryable<Product> ByMaxPrice(this IQueryable<Product> query, decimal maxPrice)
	{
		return query.Where(x => x.Price <= maxPrice);
	}

	public static IQueryable<Product> ByPriceRange(
		this IQueryable<Product> query,
		decimal minPrice,
		decimal maxPrice
	)
	{
		return query.Where(x => x.Price >= minPrice && x.Price <= maxPrice);
	}

	public static IQueryable<Product> ApplySorting(
		this IQueryable<Product> query,
		bool ascending,
		string sortBy = "id"
	)
	{
		return sortBy.ToLowerInvariant() switch
		{
			"id" => ascending ? query.OrderBy(p => p.ID) : query.OrderByDescending(p => p.ID),
			"name" => ascending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),

			"price" => ascending
				? query.OrderBy(p => p.Price)
				: query.OrderByDescending(p => p.Price),

			"createdat" => ascending
				? query.OrderBy(p => p.CreatedAt)
				: query.OrderByDescending(p => p.CreatedAt),

			_ => query.OrderBy(p => p.ID),
		};
	}

	public static IQueryable<Product> ApplySorting(
		this IQueryable<Product> query,
		FilterProductRequest req
	)
	{
		return query.ApplySorting(req.IsAscending, req.SortBy);
	}

	public static IQueryable<Product> Filter(
		this IQueryable<Product> query,
		FilterProductRequest req
	)
	{
		if (req.CategoryIDs != null && req.CategoryIDs.Length > 0)
		{
			query = query.ByCategoryIDs(req.CategoryIDs);
		}
		if (!string.IsNullOrEmpty(req.Keyword))
		{
			query = query.ByName(req.Keyword);
		}
		if (req.MinPrice.HasValue)
		{
			query = query.ByMinPrice(req.MinPrice.Value);
		}
		if (req.MaxPrice.HasValue)
		{
			query = query.ByMaxPrice(req.MaxPrice.Value);
		}
		return query;
	}
}
