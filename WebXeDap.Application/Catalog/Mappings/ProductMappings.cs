using WebXeDap.Application.Catalog.Models;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Catalog.Mappings;

internal static class ProductMappings
{
	public static ProductDto ToDto(this Product product)
	{
		return new ProductDto
		{
			Id = product.ID,
			Name = product.Name,
			Description = product.Description,
			Price = product.Price,
			CurrencySymbol = product.CurrencySymbol,
			Quantity = product.Quantity,
			Categories = product.Categories.Select(c => c.ToDto()).ToList(),
			Images = product
				.Images.OrderBy(i => i.Order)
				.Select(i => new ProductImageDto
				{
					Id = i.ID,
					Key = i.key,
					Order = i.Order,
				})
				.ToList(),
		};
	}
}
