using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebXeDap.Application.Contracts;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Features.Cart.DTOs;

namespace WebXeDap.Application.Features.Cart.Validators;

public sealed class AddCartItemValidator : AbstractValidator<AddCartItemCommand>
{
	public AddCartItemValidator(IApplicationDbContext ctx, ICurrentUserService currentUserService)
	{
		RuleFor(_ => _)
			.Must(_ => currentUserService.UserID.IsOk)
			.WithMessage("User is not authenticated.");

		RuleFor(cmd => cmd.ProductID)
			.MustAsync(
				async (productID, ct) =>
				{
					return await ctx.Products.AsNoTracking().AnyAsync(p => p.ID == productID, ct);
				}
			)
			.WithMessage("Product not found.");

		RuleFor(cmd => cmd.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0.");
	}
}

public sealed class UpdateCartItemValidator : AbstractValidator<UpdateCartItemCommand>
{
	public UpdateCartItemValidator(
		IApplicationDbContext ctx,
		ICurrentUserService currentUserService
	)
	{
		RuleFor(cmd => cmd.CartItemID)
			.MustAsync(
				async (cartItemID, ct) =>
				{
					if (!currentUserService.UserID.TryPickValue(out var userID))
					{
						return false;
					}
					return await ctx
						.CartItems.AsNoTracking()
						.AnyAsync(i => i.ID == cartItemID && i.UserID == userID, ct);
				}
			)
			.WithMessage("Cart item not found.");

		RuleFor(cmd => cmd.Quantity)
			.Cascade(CascadeMode.Stop)
			.GreaterThan(0)
			.WithMessage("Quantity must be greater than 0.");
	}
}

public sealed class DeleteCartItemValidator : AbstractValidator<int>
{
	public DeleteCartItemValidator(
		IApplicationDbContext ctx,
		ICurrentUserService currentUserService
	)
	{
		RuleFor(cmd => cmd)
			.MustAsync(
				async (cartItemID, ct) =>
				{
					if (!currentUserService.UserID.TryPickValue(out var userID))
					{
						return false;
					}
					return await ctx
						.CartItems.AsNoTracking()
						.AnyAsync(i => i.ID == cartItemID && i.UserID == userID, ct);
				}
			)
			.WithMessage("Cart item not found.");
	}
}
