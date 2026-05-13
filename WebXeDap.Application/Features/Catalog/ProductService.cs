using Microsoft.EntityFrameworkCore;
using Util.Primitives.ResultType;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Extensions;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.Application.Features.Catalog.Mapper;
using WebXeDap.Application.Features.Catalog.Queries;
using WebXeDap.Application.Features.Catalog.Validators;

namespace WebXeDap.Application.Features.Catalog;

public class ProductService : IProductService
{
	private readonly IApplicationDbContext _ctx;
	private readonly ProductMapper _mapper;
	private readonly CreateProductValidator _createProductValidator;
	private readonly UpdateProductValidator _updateProductValidator;
	private readonly DeleteProductValidator _deleteProductValidator;

	public ProductService(
		IApplicationDbContext ctx,
		ProductMapper mapper,
		CreateProductValidator createProductValidator,
		UpdateProductValidator updateProductValidator,
		DeleteProductValidator deleteProductValidator
	)
	{
		_ctx = ctx;
		_mapper = mapper;
		_createProductValidator = createProductValidator;
		_updateProductValidator = updateProductValidator;
		_deleteProductValidator = deleteProductValidator;
	}

	public Task<int> CountAsync(FilterProductCommand cmd)
	{
		return _ctx.Products.Filter(cmd).CountAsync(default);
	}

	public async Task<Result<SimpleProductResponse>> CreateAsync(CreateProductCommand cmd)
	{
		var validationRes = await _createProductValidator.ValidateAsync(cmd);
		if (!validationRes.IsValid)
		{
			return new ValidationError(validationRes.ToDictionary());
		}
		var product = _mapper.ToProduct(cmd);
		await _ctx.Products.AddAsync(product);
		var res = await _ctx.SaveChangesAsync(default);
		if (res == 0)
		{
			return new UnknownError("Failed to create product.");
		}
		return _mapper.ToSimpleProductResponse(product);
	}

	public async Task<Result> DeleteAsync(int id)
	{
		var validationRes = await _deleteProductValidator.ValidateAsync(id);
		if (!validationRes.IsValid)
		{
			return new ValidationError(validationRes.ToDictionary());
		}
		var result = await _ctx
			.Products.ByID(id)
			.SingleAsync(default)
			.ToResult(ex =>
				ex switch
				{
					InvalidOperationException => new NotFoundError("Product not found."),
					_ => new UnknownError(ex.Message),
				}
			);

		if (result.TryPickError(out var error))
		{
			return error;
		}

		_ctx.Products.Remove(result.AsT0);
		var res = await _ctx.SaveChangesAsync(default);
		if (res == 0)
		{
			return new UnknownError("Failed to delete product.");
		}
		return Result.Ok();
	}

	public async Task<List<SimpleProductResponse>> FilterAsync(
		FilterProductCommand cmd,
		int page,
		int size
	)
	{
		var products = await _ctx
			.Products.Filter(cmd)
			.ApplySorting(cmd)
			.Page(page, size)
			.ToListAsync(default);
		return [.. products.Select(_mapper.ToSimpleProductResponse)];
	}

	public async Task<Result<DetailedProductResponse>> GetByIDAsync(int id)
	{
		var product = await _ctx.Products.ByID(id).SingleAsync(default);
		if (product == null)
		{
			return new NotFoundError("Product not found.");
		}
		return _mapper.ToDetailedProductResponse(product);
	}

	public async Task<Result<DetailedProductResponse>> UpdateAsync(UpdateProductCommand req)
	{
		var validationResult = await _updateProductValidator.ValidateAsync(req);
		if (!validationResult.IsValid)
		{
			return new ValidationError(validationResult.ToDictionary());
		}
		var product = _mapper.ToProduct(req);
		_ctx.Products.Update(product);
		var result = await _ctx.SaveChangesAsync(default);
		if (result == 0)
		{
			return new UnknownError("Failed to update product.");
		}
		return _mapper.ToDetailedProductResponse(product);
	}
}
