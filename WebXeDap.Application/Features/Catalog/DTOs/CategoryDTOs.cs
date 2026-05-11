namespace WebXeDap.Application.Features.Catalog.DTOs;

public record CreateCategoryRequest(string Name, int? ParentCategoryID);

public record UpdateCategoryRequest(int ID, string Name, int? ParentCategoryID);

public record CategoryResponse(int ID, string Name, int? ParentCategoryID);

public record HierarchyCategoryResponse(
	int ID,
	string Name,
	int? ParentCategoryID,
	List<HierarchyCategoryResponse> Children
);
