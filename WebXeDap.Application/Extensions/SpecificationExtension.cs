using System.Linq.Expressions;
using Ardalis.Specification;

namespace WebXeDap.Application.Extensions;

public enum Order
{
	ASCENDING,
	DESCENDING,
}

public static class SpecificationExtensions
{
	public static ISpecificationBuilder<T> Paginate<T>(
		this ISpecificationBuilder<T> builder,
		int Page,
		int PageSize
	)
	{
		if (Page <= 0)
			throw new ArgumentOutOfRangeException(nameof(Page), "Page must be >= 1");
		if (PageSize <= 0)
			throw new ArgumentOutOfRangeException(nameof(PageSize), "PageSize must be >= 1");
		return builder.Skip((Page - 1) * PageSize).Take(PageSize);
	}

	public static ISpecificationBuilder<T> OrderBy<T>(
		this ISpecificationBuilder<T> builder,
		Expression<Func<T, object?>> expression,
		Order order = Order.ASCENDING
	)
	{
		return order switch
		{
			Order.ASCENDING => builder.OrderBy(expression),
			Order.DESCENDING => builder.OrderByDescending(expression),
			_ => throw new ArgumentException(nameof(order), "order must be 0|1"),
		};
	}
}
