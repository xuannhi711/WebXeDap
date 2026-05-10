using Riok.Mapperly.Abstractions;
using WebXeDap.Application.DTOs;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Features.Catalog.Mapper;

[Mapper]
public partial class CategoryMapper
{
	[MapperIgnoreSource(nameof(Category.ParentCategory))]
	[MapperIgnoreSource(nameof(Category.Children))]
	[MapperIgnoreSource(nameof(Category.Products))]
	public partial CategoryResponse ToCategoryResponse(Category category);

	[MapperIgnoreSource(nameof(Category.ParentCategory))]
	[MapperIgnoreSource(nameof(Category.Products))]
	public partial HierarchyCategoryResponse ToHierarchyCategoryResponse(Category category);

	[MapperIgnoreTarget(nameof(Category.ID))]
	[MapperIgnoreTarget(nameof(Category.ParentCategory))]
	[MapperIgnoreTarget(nameof(Category.Children))]
	[MapperIgnoreTarget(nameof(Category.Products))]
	public partial Category ToCategory(CreateCategoryRequest request);

	[MapperIgnoreTarget(nameof(Category.ParentCategory))]
	[MapperIgnoreTarget(nameof(Category.Children))]
	[MapperIgnoreTarget(nameof(Category.Products))]
	public partial Category ToCategory(UpdateCategoryRequest request);

	[MapperIgnoreTarget(nameof(Category.ParentCategory))]
	[MapperIgnoreTarget(nameof(Category.Children))]
	[MapperIgnoreTarget(nameof(Category.Products))]
	public partial void PatchCategory(
		UpdateCategoryRequest request,
		[MappingTarget] Category category
	);
}
