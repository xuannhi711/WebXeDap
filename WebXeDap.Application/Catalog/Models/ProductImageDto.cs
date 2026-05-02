namespace WebXeDap.Application.Catalog.Models;

public sealed record ProductImageDto
{
	public int Id { get; init; }
	public string Key { get; init; } = string.Empty;
	public int Order { get; init; }
}
