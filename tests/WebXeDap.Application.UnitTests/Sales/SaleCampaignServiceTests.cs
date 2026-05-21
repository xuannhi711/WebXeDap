using Microsoft.EntityFrameworkCore;
using Util.Primitives.ResultType;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Features.Sales.DTOs;
using WebXeDap.Application.UnitTests.Extensions;
using WebXeDap.Domain.Enums;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.UnitTests.Sales;

[Trait("Category", "Unit")]
public class SaleCampaignServiceCreateTests
{
	private readonly IApplicationDbContext ctx;
	private readonly ISaleCampaignService service;

	public SaleCampaignServiceCreateTests()
	{
		var fixture = new ApplicationTestFixture();
		ctx = fixture.GetService<IApplicationDbContext>();
		service = fixture.GetService<ISaleCampaignService>();
	}

	[Fact]
	public async Task CreateAsync_Pass_WhenRequestIsValid()
	{
		var product1 = await ctx.AddRandomProductAsync();
		var product2 = await ctx.AddRandomProductAsync();
		var now = DateTime.UtcNow;
		var cmd = new CreateSaleCampaignCommand(
			Name: "Summer Sale",
			Description: "Limited time",
			DiscountType: DiscountType.Percentage,
			DiscountValue: 10,
			StartsAt: now.AddDays(-1),
			EndsAt: now.AddDays(1),
			ProductIDs: [product1.ID, product2.ID]
		);

		var result = await service.CreateAsync(cmd);

		Assert.True(result.TryPickValue(out var created));
		Assert.Equal(cmd.Name, created.Name);
		Assert.Equal(cmd.Description, created.Description);
		Assert.Equal(cmd.DiscountType, created.DiscountType);
		Assert.Equal(cmd.DiscountValue, created.DiscountValue);
		Assert.Equal(cmd.StartsAt, created.StartsAt);
		Assert.Equal(cmd.EndsAt, created.EndsAt);
		Assert.True(created.IsActive);
		Assert.Contains(product1.ID, created.ProductIDs);
		Assert.Contains(product2.ID, created.ProductIDs);

		var stored = await ctx
			.SaleCampaigns.AsNoTracking()
			.FirstOrDefaultAsync(s => s.ID == created.ID);
		Assert.NotNull(stored);
	}

	[Fact]
	public async Task CreateAsync_Fail_WhenRequestIsInvalid()
	{
		var now = DateTime.UtcNow;
		var cmd = new CreateSaleCampaignCommand(
			Name: "",
			Description: null,
			DiscountType: DiscountType.Percentage,
			DiscountValue: 0,
			StartsAt: now.AddDays(1),
			EndsAt: now.AddDays(-1),
			ProductIDs: [Random.Shared.Next()]
		);

		var result = await service.CreateAsync(cmd);

		Assert.True(result.TryPickError(out var error));
		var validationError = Assert.IsType<ValidationError>(error);
		Assert.True(validationError.Errors.ContainsKey(nameof(CreateSaleCampaignCommand.Name)));
		Assert.True(
			validationError.Errors.ContainsKey(nameof(CreateSaleCampaignCommand.DiscountValue))
		);
		Assert.True(validationError.Errors.ContainsKey(nameof(CreateSaleCampaignCommand.StartsAt)));
		Assert.True(
			validationError.Errors.ContainsKey(nameof(CreateSaleCampaignCommand.ProductIDs))
		);
	}
}

[Trait("Category", "Unit")]
public class SaleCampaignServiceReadTests
{
	private readonly IApplicationDbContext ctx;
	private readonly ISaleCampaignService service;

	public SaleCampaignServiceReadTests()
	{
		var fixture = new ApplicationTestFixture();
		ctx = fixture.GetService<IApplicationDbContext>();
		service = fixture.GetService<ISaleCampaignService>();
	}

	[Fact]
	public async Task GetByIDAsync_Pass_WhenCampaignExists()
	{
		var product = await ctx.AddRandomProductAsync();
		var now = DateTime.UtcNow;
		var campaign = new SaleCampaign
		{
			Name = "Weekend Deal",
			DiscountType = DiscountType.FixedAmount,
			DiscountValue = 50,
			StartsAt = now.AddDays(-1),
			EndsAt = now.AddDays(1),
			Products = [product],
		};
		await ctx.SaleCampaigns.AddAsync(campaign);
		await ctx.SaveChangesAsync(default);

		var result = await service.GetByIDAsync(campaign.ID);

		Assert.True(result.TryPickValue(out var found));
		Assert.Equal(campaign.ID, found.ID);
		Assert.Equal(campaign.Name, found.Name);
		Assert.Contains(product.ID, found.ProductIDs);
		Assert.True(found.IsActive);
	}

	[Fact]
	public async Task GetByIDAsync_Fail_WhenCampaignDoesNotExist()
	{
		var result = await service.GetByIDAsync(Random.Shared.Next());

		Assert.True(result.TryPickError(out var error));
		Assert.IsType<NotFoundError>(error);
	}

	[Fact]
	public async Task ListAsync_Filters_ActiveOnly()
	{
		var product = await ctx.AddRandomProductAsync();
		var now = DateTime.UtcNow;
		var active = new SaleCampaign
		{
			Name = "Active",
			DiscountType = DiscountType.Percentage,
			DiscountValue = 10,
			StartsAt = now.AddDays(-1),
			EndsAt = now.AddDays(1),
			Products = [product],
		};
		var inactive = new SaleCampaign
		{
			Name = "Inactive",
			DiscountType = DiscountType.Percentage,
			DiscountValue = 15,
			StartsAt = now.AddDays(-10),
			EndsAt = now.AddDays(-5),
			Products = [product],
		};
		await ctx.SaleCampaigns.AddRangeAsync(active, inactive);
		await ctx.SaveChangesAsync(default);

		var result = await service.ListAsync(activeOnly: true);

		Assert.Single(result);
		Assert.Equal(active.Name, result[0].Name);
	}
}

[Trait("Category", "Unit")]
public class SaleCampaignServiceUpdateTests
{
	private readonly IApplicationDbContext ctx;
	private readonly ISaleCampaignService service;

	public SaleCampaignServiceUpdateTests()
	{
		var fixture = new ApplicationTestFixture();
		ctx = fixture.GetService<IApplicationDbContext>();
		service = fixture.GetService<ISaleCampaignService>();
	}

	[Fact]
	public async Task UpdateAsync_Pass_WhenRequestIsValid()
	{
		var product1 = await ctx.AddRandomProductAsync();
		var product2 = await ctx.AddRandomProductAsync();
		var now = DateTime.UtcNow;
		var campaign = new SaleCampaign
		{
			Name = "Old Name",
			DiscountType = DiscountType.FixedAmount,
			DiscountValue = 20,
			StartsAt = now.AddDays(-3),
			EndsAt = now.AddDays(3),
			Products = [product1],
		};
		await ctx.SaleCampaigns.AddAsync(campaign);
		await ctx.SaveChangesAsync(default);

		var cmd = new UpdateSaleCampaignCommand(
			Name: "Updated",
			Description: "Updated description",
			DiscountType: DiscountType.Percentage,
			DiscountValue: 15,
			StartsAt: now.AddDays(-2),
			EndsAt: now.AddDays(2),
			ProductIDs: [product2.ID]
		);

		var result = await service.UpdateAsync(campaign.ID, cmd);

		Assert.True(result.TryPickValue(out var updated));
		Assert.Equal(cmd.Name, updated.Name);
		Assert.Equal(cmd.Description, updated.Description);
		Assert.Equal(cmd.DiscountType, updated.DiscountType);
		Assert.Equal(cmd.DiscountValue, updated.DiscountValue);
		Assert.Equal(cmd.StartsAt, updated.StartsAt);
		Assert.Equal(cmd.EndsAt, updated.EndsAt);
		Assert.Single(updated.ProductIDs);
		Assert.Equal(product2.ID, updated.ProductIDs.First());
	}

	[Fact]
	public async Task UpdateAsync_Fail_WhenIDIsInvalid()
	{
		var cmd = new UpdateSaleCampaignCommand(
			Name: "Updated",
			Description: null,
			DiscountType: DiscountType.FixedAmount,
			DiscountValue: 10,
			StartsAt: DateTime.UtcNow.AddDays(-1),
			EndsAt: DateTime.UtcNow.AddDays(1),
			ProductIDs: null
		);

		var result = await service.UpdateAsync(Random.Shared.Next(), cmd);

		Assert.True(result.TryPickError(out var error));
		Assert.IsType<NotFoundError>(error);
	}
}

[Trait("Category", "Unit")]
public class SaleCampaignServiceDeleteTests
{
	private readonly IApplicationDbContext ctx;
	private readonly ISaleCampaignService service;

	public SaleCampaignServiceDeleteTests()
	{
		var fixture = new ApplicationTestFixture();
		ctx = fixture.GetService<IApplicationDbContext>();
		service = fixture.GetService<ISaleCampaignService>();
	}

	[Fact]
	public async Task DeleteAsync_RemovesCampaign()
	{
		var product = await ctx.AddRandomProductAsync();
		var now = DateTime.UtcNow;
		var campaign = new SaleCampaign
		{
			Name = "To Delete",
			DiscountType = DiscountType.FixedAmount,
			DiscountValue = 10,
			StartsAt = now.AddDays(-1),
			EndsAt = now.AddDays(1),
			Products = [product],
		};
		await ctx.SaleCampaigns.AddAsync(campaign);
		await ctx.SaveChangesAsync(default);

		var result = await service.DeleteAsync(campaign.ID);

		Assert.True(result.IsOk);
		var stored = await ctx.SaleCampaigns.FirstOrDefaultAsync(
			s => s.ID == campaign.ID && !s.IsDeleted,
			default
		);
		Assert.Null(stored);
	}
}
