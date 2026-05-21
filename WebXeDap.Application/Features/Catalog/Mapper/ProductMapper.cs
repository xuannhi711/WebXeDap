using Riok.Mapperly.Abstractions;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Features.Catalog.Mapper;

[Mapper(AllowNullPropertyAssignment = false)]
public partial class ProductMapper
{
	private readonly IApplicationDbContext ctx;

	[UseMapper]
	private readonly ProductImageMapper productImageMapper;

	[UseMapper]
	private readonly CategoryMapper categoryMapper;

	public ProductMapper(
		IApplicationDbContext ctx,
		ProductImageMapper productImageMapper,
		CategoryMapper categoryMapper
	)
	{
		this.ctx = ctx;
		this.productImageMapper = productImageMapper;
		this.categoryMapper = categoryMapper;
	}

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
	[MapProperty(nameof(CreateProductCommand.CategoryIDs), nameof(Product.Categories))]
	public partial Product ToProduct(CreateProductCommand request);

	[MapperIgnoreTarget(nameof(Product.Images))]
	[MapProperty(nameof(UpdateProductCommand.CategoryIDs), nameof(Product.Categories))]
	public partial void PatchProduct(UpdateProductCommand cmd, [MappingTarget] Product product);
}
