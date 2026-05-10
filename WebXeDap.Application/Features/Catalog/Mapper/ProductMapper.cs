using Riok.Mapperly.Abstractions;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.DTOs;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Features.Catalog.Mapper;

[Mapper]
public partial class ProductMapper
{
	[MapperIgnoreSource(nameof(Product.Categories))]
	[MapProperty(nameof(Product.Images), nameof(SimpleProductResponse.Image))]
	public partial SimpleProductResponse ToSimpleProductResponse(Product product);

	public partial DetailedProductResponse ToDetailedProductResponse(Product product);

	[MapperIgnoreTarget(nameof(Product.ID))]
	[MapperIgnoreTarget(nameof(Product.Images))]
	[MapperIgnoreTarget(nameof(Product.CreatedAt))]
	[MapperIgnoreTarget(nameof(Product.UpdatedAt))]
	[MapperIgnoreTarget(nameof(Product.IsDeleted))]
	[MapperIgnoreTarget(nameof(Product.DeletedAt))]
	[MapProperty(nameof(CreateProductRequest.CategoryIDs), nameof(Product.Categories))]
	public partial Product ToProduct(CreateProductRequest request);

	[MapperIgnoreTarget(nameof(Product.Images))]
	[MapProperty(nameof(UpdateProductRequest.CategoryIDs), nameof(Product.Categories))]
	public partial Product ToProduct(UpdateProductRequest request);

	[UserMapping]
	public ICollection<Category> CategoryIDsToCategories(int[]? categoryIDs)
	{
		if (categoryIDs == null)
		{
			return [];
		}
		return [.. categoryIDs.Select(id => new Category { ID = id, Name = "" })];
	}

	[UserMapping]
	public CategoryResponse CategoryToCategoryResponse(Category category)
	{
		return new CategoryResponse(category.ID, category.Name, category.ParentCategoryID);
	}

	[UserMapping]
	public ProductImageResponse ProductImageToProductImageResponse(ProductImage image)
	{
		return new ProductImageResponse(image.ID, image.Key, image.Order);
	}

	[UserMapping]
	public ProductImageResponse? GetOpeningImage(ICollection<ProductImage> images)
	{
		if (!images.Any())
		{
			return null;
		}
		var openingImg = images.OrderBy(i => i.Order).First();
		return ProductImageMapper.ToProductImageResponse(openingImg);
	}
}
