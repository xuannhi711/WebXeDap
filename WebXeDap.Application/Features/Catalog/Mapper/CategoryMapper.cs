using Riok.Mapperly.Abstractions;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Features.Catalog.Mapper;

[Mapper(AllowNullPropertyAssignment = false)]
public partial class CategoryMapper
{
	private readonly IApplicationDbContext ctx;

	public CategoryMapper(IApplicationDbContext ctx)
	{
		this.ctx = ctx;
	}

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
	public partial Category ToCategory(CreateCategoryCommand request);

	[MapperIgnoreTarget(nameof(Category.ParentCategory))]
	[MapperIgnoreTarget(nameof(Category.Children))]
	[MapperIgnoreTarget(nameof(Category.Products))]
	public partial void PatchCategory(
		UpdateCategoryCommand request,
		[MappingTarget] Category category
	);

	[UserMapping]
	public ICollection<Category> CategoryIDsToCategories(ICollection<int>? categoryIDs)
	{
		if (categoryIDs == null)
		{
			return [];
		}
		return [.. ctx.Categories.Where(c => categoryIDs.Contains(c.ID))];
	}
}
