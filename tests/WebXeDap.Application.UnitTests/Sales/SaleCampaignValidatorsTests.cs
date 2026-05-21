using FluentValidation.TestHelper;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Features.Sales.DTOs;
using WebXeDap.Application.Features.Sales.Validators;
using WebXeDap.Application.UnitTests.Extensions;
using WebXeDap.Domain.Enums;

namespace WebXeDap.Application.UnitTests.Sales;

[Trait("Category", "Unit")]
public sealed class CreateSaleCampaignValidatorTests
{
	private readonly CreateSaleCampaignValidator validator;
	private readonly IApplicationDbContext ctx;

	public CreateSaleCampaignValidatorTests()
	{
		var fixture = new ApplicationTestFixture();
		ctx = fixture.GetService<IApplicationDbContext>();
		validator = fixture.GetService<CreateSaleCampaignValidator>();
	}

	[Fact]
	public async Task CreateSaleCampaignValidator_Pass_WhenRequestIsValid()
	{
		var product = await ctx.AddRandomProductAsync();
		var now = DateTime.UtcNow;
		var cmd = new CreateSaleCampaignCommand(
			Name: "Valid",
			Description: null,
			DiscountType: DiscountType.Percentage,
			DiscountValue: 20,
			StartsAt: now.AddDays(-1),
			EndsAt: now.AddDays(1),
			ProductIDs: [product.ID]
		);

		var result = await validator.TestValidateAsync(cmd);

		result.ShouldNotHaveAnyValidationErrors();
	}

	[Fact]
	public async Task CreateSaleCampaignValidator_Fail_WhenRequestIsInvalid()
	{
		var now = DateTime.UtcNow;
		var cmd = new CreateSaleCampaignCommand(
			Name: "",
			Description: null,
			DiscountType: DiscountType.Percentage,
			DiscountValue: 150,
			StartsAt: now.AddDays(1),
			EndsAt: now.AddDays(-1),
			ProductIDs: [Random.Shared.Next()]
		);

		var result = await validator.TestValidateAsync(cmd);

		result.ShouldHaveValidationErrors();
		result.ShouldHaveValidationErrorFor(c => c.Name);
		result.ShouldHaveValidationErrorFor(c => c.DiscountValue);
		result.ShouldHaveValidationErrorFor(c => c.StartsAt);
		result.ShouldHaveValidationErrorFor(c => c.ProductIDs);
	}
}

[Trait("Category", "Unit")]
public sealed class UpdateSaleCampaignValidatorTests
{
	private readonly UpdateSaleCampaignValidator validator;
	private readonly IApplicationDbContext ctx;

	public UpdateSaleCampaignValidatorTests()
	{
		var fixture = new ApplicationTestFixture();
		ctx = fixture.GetService<IApplicationDbContext>();
		validator = fixture.GetService<UpdateSaleCampaignValidator>();
	}

	[Fact]
	public async Task UpdateSaleCampaignValidator_Pass_WhenRequestIsValid()
	{
		var product = await ctx.AddRandomProductAsync();
		var now = DateTime.UtcNow;
		var cmd = new UpdateSaleCampaignCommand(
			Name: "Updated",
			Description: "Updated",
			DiscountType: DiscountType.FixedAmount,
			DiscountValue: 50,
			StartsAt: now.AddDays(-2),
			EndsAt: now.AddDays(2),
			ProductIDs: [product.ID]
		);

		var result = await validator.TestValidateAsync(cmd);

		result.ShouldNotHaveAnyValidationErrors();
	}

	[Fact]
	public async Task UpdateSaleCampaignValidator_Fail_WhenRequestIsInvalid()
	{
		var now = DateTime.UtcNow;
		var cmd = new UpdateSaleCampaignCommand(
			Name: "",
			Description: null,
			DiscountType: DiscountType.Percentage,
			DiscountValue: 0,
			StartsAt: now.AddDays(2),
			EndsAt: now.AddDays(1),
			ProductIDs: [Random.Shared.Next()]
		);

		var result = await validator.TestValidateAsync(cmd);

		result.ShouldHaveValidationErrors();
		result.ShouldHaveValidationErrorFor(c => c.Name);
		result.ShouldHaveValidationErrorFor(c => c.DiscountValue);
		result.ShouldHaveValidationErrorFor(c => c.StartsAt);
		result.ShouldHaveValidationErrorFor(c => c.ProductIDs);
	}
}
