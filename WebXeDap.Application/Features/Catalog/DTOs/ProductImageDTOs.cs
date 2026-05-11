namespace WebXeDap.Application.Features.Catalog.DTOs;

public record CreateProductImageRequest(int ProductID, string Key, int Order);

public record ProductImageResponse(int ID, string URL, int Order);
