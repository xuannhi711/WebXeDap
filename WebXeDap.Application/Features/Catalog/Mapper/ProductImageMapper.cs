using Riok.Mapperly.Abstractions;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Features.Catalog.Mapper;

[Mapper]
public partial class ProductImageMapper
{
	[MapperIgnoreSource(nameof(ProductImage.Product))]
	[MapperIgnoreSource(nameof(ProductImage.ProductID))]
	[MapProperty(nameof(ProductImage.Key), nameof(ProductImageResponse.URL))]
	public partial ProductImageResponse ToProductImageResponse(ProductImage img);

	[UserMapping]
	public ProductImageResponse? GetOpeningImage(ICollection<ProductImage> images)
	{
		if (images == null || images.Count == 0)
		{
			return null;
		}
		return images.OrderBy(i => i.Order).FirstOrDefault() switch
		{
			null => null,
			var img => ToProductImageResponse(img),
		};
	}
}
