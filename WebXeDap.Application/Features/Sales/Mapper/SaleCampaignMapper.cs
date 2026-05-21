using Riok.Mapperly.Abstractions;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Features.Sales.DTOs;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Features.Sales.Mapper;

[Mapper(AllowNullPropertyAssignment = false)]
public partial class SaleCampaignMapper
{
	private readonly IApplicationDbContext ctx;

	public SaleCampaignMapper(IApplicationDbContext ctx)
	{
		this.ctx = ctx;
	}

	[MapperIgnoreTarget(nameof(SaleCampaign.ID))]
	[MapperIgnoreTarget(nameof(SaleCampaign.CreatedAt))]
	[MapperIgnoreTarget(nameof(SaleCampaign.UpdatedAt))]
	[MapperIgnoreTarget(nameof(SaleCampaign.DeletedAt))]
	[MapProperty(nameof(CreateSaleCampaignCommand.ProductIDs), nameof(SaleCampaign.Products))]
	public partial SaleCampaign ToSaleCampaign(CreateSaleCampaignCommand cmd);

	[MapperIgnoreTarget(nameof(SaleCampaign.ID))]
	[MapperIgnoreTarget(nameof(SaleCampaign.CreatedAt))]
	[MapperIgnoreTarget(nameof(SaleCampaign.UpdatedAt))]
	[MapperIgnoreTarget(nameof(SaleCampaign.IsDeleted))]
	[MapperIgnoreTarget(nameof(SaleCampaign.DeletedAt))]
	[MapProperty(nameof(UpdateSaleCampaignCommand.ProductIDs), nameof(SaleCampaign.Products))]
	public partial void PatchSaleCampaign(
		UpdateSaleCampaignCommand cmd,
		[MappingTarget] SaleCampaign campaign
	);

	[MapPropertyFromSource(nameof(SaleCampaignResponse.IsActive), Use = nameof(MapIsActive))]
	[MapProperty(nameof(SaleCampaign.Products), nameof(SaleCampaignResponse.ProductIDs))]
	public partial SaleCampaignResponse ToSaleCampaignResponse(SaleCampaign campaign);

	[UserMapping]
	public Product ProductIDToProduct(int productID)
	{
		return ctx.Products.First(p => p.ID == productID);
	}

	[UserMapping]
	public ICollection<int> ProductsToProductIDs(ICollection<Product> products)
	{
		return [.. products.Select(p => p.ID)];
	}

	[UserMapping]
	private bool MapIsActive(SaleCampaign campaign)
	{
		return campaign.IsActive(DateTime.UtcNow);
	}
}
