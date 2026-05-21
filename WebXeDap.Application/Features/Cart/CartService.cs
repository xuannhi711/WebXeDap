using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Util.Primitives.ResultType;
using WebXeDap.Application.Contracts;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Extensions;
using WebXeDap.Application.Features.Cart.DTOs;
using WebXeDap.Application.Features.Cart.Mapper;

namespace WebXeDap.Application.Features.Cart;

public class CartService : ICartService
{
	private readonly IApplicationDbContext ctx;
	private readonly ICurrentUserService currentUserService;
	private readonly CartMapper mapper;
	private readonly IValidator<AddCartItemCommand> addValidator;
	private readonly IValidator<UpdateCartItemCommand> updateValidator;
	private readonly IValidator<int> deleteValidator;

	public CartService(
		IApplicationDbContext ctx,
		CartMapper mapper,
		ICurrentUserService currentUserService,
		IValidator<AddCartItemCommand> addValidator,
		IValidator<UpdateCartItemCommand> updateValidator,
		IValidator<int> deleteValidator
	)
	{
		this.ctx = ctx;
		this.mapper = mapper;
		this.currentUserService = currentUserService;
		this.addValidator = addValidator;
		this.updateValidator = updateValidator;
		this.deleteValidator = deleteValidator;
	}

	public async Task<Result<List<CartItemResponse>>> ListAsync()
	{
		if (!currentUserService.UserID.TryPickValue(out var userID))
		{
			return new UnauthorizedError("User is not authenticated.");
		}
		var items = await ctx
			.CartItems.AsNoTracking()
			.Include(i => i.Product)
			.Where(i => i.UserID == userID)
			.ToListAsync(default);
		return mapper.ToCartItemResponseList(items);
	}

	public async Task<Result<CartItemResponse>> AddAsync(AddCartItemCommand cmd)
	{
		if (!currentUserService.UserID.TryPickValue(out var userID))
		{
			return new UnauthorizedError("User is not authenticated.");
		}
		var validationResult = await addValidator.ValidateAsync(cmd);
		if (!validationResult.IsValid)
		{
			return validationResult.ToValidationError();
		}

		var cartItem = await ctx
			.CartItems.Include(i => i.Product)
			.FirstOrDefaultAsync(i => i.UserID == userID && i.ProductID == cmd.ProductID, default);

		if (cartItem is not null)
		{
			cartItem.Quantity += cmd.Quantity;
			ctx.CartItems.Update(cartItem);
			await ctx.SaveChangesAsync(default);
			return mapper.ToCartItemResponse(cartItem);
		}

		var newCartItem = mapper.ToCartItem(cmd, userID);
		await ctx.CartItems.AddAsync(newCartItem, default);
		await ctx.CartItems.Entry(newCartItem).Reference(i => i.Product).LoadAsync();
		await ctx.SaveChangesAsync(default);
		return mapper.ToCartItemResponse(newCartItem);
	}

	public async Task<Result<CartItemResponse>> UpdateAsync(UpdateCartItemCommand cmd)
	{
		if (!currentUserService.UserID.TryPickValue(out var userID))
		{
			return new UnauthorizedError("User is not authenticated.");
		}

		var validationResult = await updateValidator.ValidateAsync(cmd);
		if (!validationResult.IsValid)
		{
			return validationResult.ToValidationError();
		}

		var cartItem = await ctx
			.CartItems.Include(i => i.Product)
			.FirstAsync(i => i.UserID == userID && i.ID == cmd.CartItemID, default);

		mapper.PatchCartItem(cmd, cartItem);
		ctx.CartItems.Update(cartItem);
		await ctx.SaveChangesAsync(default);
		return mapper.ToCartItemResponse(cartItem);
	}

	public async Task<Result> DeleteAsync(int cartItemID)
	{
		if (!currentUserService.UserID.TryPickValue(out var userID))
		{
			return new UnauthorizedError("User is not authenticated.");
		}
		var validationResult = await deleteValidator.ValidateAsync(cartItemID);
		if (!validationResult.IsValid)
		{
			return validationResult.ToValidationError();
		}

		var cartItem = await ctx.CartItems.FirstAsync(
			i => i.UserID == userID && i.ID == cartItemID,
			default
		);

		ctx.CartItems.Remove(cartItem);
		await ctx.SaveChangesAsync(default);
		return Result.Ok();
	}
}
