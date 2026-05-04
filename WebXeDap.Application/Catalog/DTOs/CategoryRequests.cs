namespace WebXeDap.Application.Catalog.DTOs;

public record CreateCategoryRequest(
    string Name,
    int? ParentCategoryID
);

public record UpdateCategoryRequest(
    int ID,
    string Name,
    int? ParentCategoryID
);

public record DeleteCategoryRequest(
    int ID
);