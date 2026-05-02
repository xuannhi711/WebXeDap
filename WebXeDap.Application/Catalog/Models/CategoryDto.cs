namespace WebXeDap.Application.Catalog.Models;

public sealed record CategoryDto
{
	public int Id { get; init; }
	public string Name { get; init; } = string.Empty;
	public int? ParentCategoryId { get; init; }
}
