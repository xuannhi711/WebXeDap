namespace WebXeDap.Application.Features.Catalog.DTOs;

public record CreateCategoryRequest(string Name, int? ParentCategoryID);

public record UpdateCategoryRequest(int ID, string Name, int? ParentCategoryID);

public record DeleteCategoryRequest(int ID);

public record CategoryResponse(int ID, string Name, int? ParentCategoryID);

public record CategoryWithSubcategoriesResponse(
	int ID,
	string Name,
	int? ParentCategoryID,
	List<CategoryWithSubcategoriesResponse> Children
);
