using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Features.Cart.DTOs;
using WebXeDap.Application.Features.Cart.Validators;
using WebXeDap.Application.UnitTests.Extensions;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.UnitTests.Cart;

[Trait("Category", "Unit")]
public sealed class AddCartItemValidatorTests
{
	private readonly AddCartItemValidator validator;
	private readonly IApplicationDbContext ctx;
	private readonly TestCurrentUserService currentUser;

	public AddCartItemValidatorTests()
	{
		var fixture = new ApplicationTestFixture();
		ctx = fixture.GetService<IApplicationDbContext>();
		currentUser = fixture.GetService<TestCurrentUserService>();
		validator = fixture.GetService<AddCartItemValidator>();
	}

	[Fact]
	public async Task AddCartItemValidator_Pass_WhenRequestIsValid()
	{
		var user = await ctx.AddRandomUserAsync();
		currentUser.UserID = user.Id;
		var product = await ctx.AddRandomProductAsync();

		var req = new AddCartItemCommand(ProductID: product.ID, Quantity: 2);

		var result = await validator.TestValidateAsync(req);

		result.ShouldNotHaveAnyValidationErrors();
	}

	[Fact]
	public async Task AddCartItemValidator_Fail_WhenRequestIsInvalid()
	{
		var req = new AddCartItemCommand(ProductID: Random.Shared.Next(), Quantity: 0);

		var result = await validator.TestValidateAsync(req);

		result.ShouldHaveValidationErrors();
		result.ShouldHaveValidationErrorFor(r => r).WithErrorMessage("User is not authenticated.");
		result.ShouldHaveValidationErrorFor(r => r.ProductID);
		result.ShouldHaveValidationErrorFor(r => r.Quantity);
	}
}

[Trait("Category", "Unit")]
public sealed class UpdateCartItemValidatorTests
{
	private readonly UpdateCartItemValidator validator;
	private readonly IApplicationDbContext ctx;
	private readonly TestCurrentUserService currentUser;

	public UpdateCartItemValidatorTests()
	{
		var fixture = new ApplicationTestFixture();
		ctx = fixture.GetService<IApplicationDbContext>();
		currentUser = fixture.GetService<TestCurrentUserService>();
		validator = fixture.GetService<UpdateCartItemValidator>();
	}

	[Fact]
	public async Task UpdateCartItemValidator_Pass_WhenRequestIsValid()
	{
		var user = await ctx.AddRandomUserAsync();
		currentUser.UserID = user.Id;
		var product = await ctx.AddRandomProductAsync();

		var toUpdateCartItem = new CartItem
		{
			UserID = user.Id,
			User = user,
			ProductID = product.ID,
			Product = product,
			Quantity = 1,
		};
		await ctx.AddCartItemAsync(toUpdateCartItem);

		var req = new UpdateCartItemCommand(Quantity: 233);

		var result = await validator.TestValidateAsync(req);

		result.ShouldNotHaveAnyValidationErrors();
	}

	[Fact]
	public async Task UpdateCartItemValidator_Fail_WhenRequestIsInvalid()
	{
		var user = await ctx.AddRandomUserAsync();
		currentUser.UserID = user.Id;

		var cmd = new UpdateCartItemCommand(Quantity: 0);
		var result = await validator.TestValidateAsync(cmd);

		result.ShouldHaveValidationErrors();
		result.ShouldHaveValidationErrorFor(r => r.Quantity);
	}
}
