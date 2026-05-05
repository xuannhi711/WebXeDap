using Riok.Mapperly.Abstractions;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Features.Catalog.Mapper;

[Mapper]
public partial class CategoryMapper
{
    public partial CategoryResponse CategoryToCategoryResponse(Category category);

    public partial HierarchyCategoryResponse CategoryToHierarchyCategoryResponse(Category category);

    public partial Category CategoryCreateRequestToCategory(CreateCategoryRequest request);

    public partial Category CategoryUpdateRequestToCategory(UpdateCategoryRequest request);

    [MapValue(nameof(Category.Name), "")]
    public partial Category CategoryDeleteRequestToCategory(DeleteCategoryRequest request);
}