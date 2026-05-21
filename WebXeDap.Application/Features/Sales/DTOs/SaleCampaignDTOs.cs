using WebXeDap.Domain.Enums;

namespace WebXeDap.Application.Features.Sales.DTOs;

public record CreateSaleCampaignCommand(
	string Name,
	string? Description,
	DiscountType DiscountType,
	decimal DiscountValue,
	DateTime StartsAt,
	DateTime EndsAt,
	ICollection<int> ProductIDs,
	bool IsDeleted = true
);

public record UpdateSaleCampaignCommand(
	string? Name,
	string? Description,
	DiscountType? DiscountType,
	decimal? DiscountValue,
	DateTime? StartsAt,
	DateTime? EndsAt,
	ICollection<int>? ProductIDs
);

public sealed record SaleCampaignResponse
{
	public int ID { get; init; }
	public string Name { get; init; } = string.Empty;
	public string? Description { get; init; }
	public DiscountType DiscountType { get; init; }
	public decimal DiscountValue { get; init; }
	public DateTime StartsAt { get; init; }
	public DateTime EndsAt { get; init; }
	public bool IsDeleted { get; init; }
	public bool IsActive { get; init; }
	public ICollection<int> ProductIDs { get; init; } = [];
}
