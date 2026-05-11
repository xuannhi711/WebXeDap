using Riok.Mapperly.Abstractions;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Features.Catalog.Mapper;

[Mapper]
public static partial class ProductImageMapper
{
	[MapperIgnoreSource(nameof(ProductImage.Product))]
	[MapperIgnoreSource(nameof(ProductImage.ProductID))]
	[MapProperty(nameof(ProductImage.Key), nameof(ProductImageResponse.URL))]
	public static partial ProductImageResponse ToProductImageResponse(ProductImage img);
}
