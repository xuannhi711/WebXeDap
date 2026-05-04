using WebXeDap.Application.Catalog.Models;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Catalog.Mappings;

internal static class CategoryMappings
{
	public static CategoryDto ToDto(this Category category)
	{
		return new CategoryDto
		{
			Id = category.ID,
			Name = category.Name,
			ParentCategoryId = category.ParentCategoryID,
		};
	}
}
