namespace WebXeDap.Application.Features.Catalog.DTOs;

public record CreateCategoryCommand(string Name, int? ParentCategoryID);

public record UpdateCategoryCommand(int ID, string? Name, int? ParentCategoryID);

public record CategoryResponse(int ID, string Name, int? ParentCategoryID);

public record HierarchyCategoryResponse(
	int ID,
	string Name,
	int? ParentCategoryID,
	List<HierarchyCategoryResponse> Children
);
