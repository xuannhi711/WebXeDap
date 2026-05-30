using Microsoft.EntityFrameworkCore;
using Util.Primitives.ResultType;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Extensions;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.Application.Features.Catalog.Mapper;
using WebXeDap.Application.Features.Catalog.Queries;
using WebXeDap.Application.Features.Catalog.Validators;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Features.Catalog;

public class ProductService : IProductService
{
	private readonly IApplicationDbContext ctx;
	private readonly ProductMapper mapper;
	private readonly CreateProductValidator createProductValidator;
	private readonly UpdateProductValidator updateProductValidator;
	private readonly ProductImageMapper imageMapper;

	public ProductService(
		IApplicationDbContext ctx,
		ProductMapper mapper,
		CreateProductValidator createProductValidator,
		UpdateProductValidator updateProductValidator,
		ProductImageMapper imageMapper
	)
	{
		this.ctx = ctx;
		this.mapper = mapper;
		this.createProductValidator = createProductValidator;
		this.updateProductValidator = updateProductValidator;
		this.imageMapper = imageMapper;
	}

	public Task<int> CountAsync(FilterProductCommand cmd)
	{
		return ctx.Products.Filter(cmd).CountAsync(default);
	}

	public async Task<Result<SimpleProductResponse>> CreateAsync(CreateProductCommand cmd)
	{
		var validationRes = await createProductValidator.ValidateAsync(cmd);
		if (!validationRes.IsValid)
		{
			return new ValidationError(validationRes.ToDictionary());
		}
		var product = mapper.ToProduct(cmd);
		await ctx.Products.AddAsync(product);
		var res = await ctx.SaveChangesAsync(default);
		if (res == 0)
		{
			return new UnknownError("Failed to create product.");
		}
		return mapper.ToSimpleProductResponse(product);
	}

	public async Task<Result> DeleteAsync(int id)
	{
		var product = await ctx.Products.FindAsync(id);
		if (product is null)
		{
			return new NotFoundError("Product not found.");
		}

		ctx.Products.Remove(product);
		var res = await ctx.SaveChangesAsync(default);
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
		var products = await ctx
			.Products.Filter(cmd)
			.ApplySorting(cmd)
			.Page(page, size)
			.ToListAsync(default);
		return [.. products.Select(mapper.ToSimpleProductResponse)];
	}

	public async Task<Result<DetailedProductResponse>> GetByIDAsync(int id)
	{
		var product = await ctx
			.Products.Include(p => p.Images).Include(p => p.Categories)
			.FirstOrDefaultAsync(p => p.ID == id);
		if (product == null)
		{
			return new NotFoundError("Product not found.");
		}
		return mapper.ToDetailedProductResponse(product);
	}

	public async Task<List<ProductImageResponse>> ListImagesAsync(int productId)
	{
		var images = await ctx
			.ProductImages.Where(i => i.ProductID == productId)
			.OrderBy(i => i.Order)
			.ToListAsync();
		return images.Select(imageMapper.ToProductImageResponse).ToList();
	}

	public async Task<Result<ProductImageResponse>> AddImageAsync(CreateProductImageCommand cmd)
	{
		var product = await ctx.Products.FindAsync(cmd.ProductID);
		if (product == null)
			return new NotFoundError("Product not found.");

		var img = new ProductImage
		{
			Key = cmd.Key,
			Order = cmd.Order,
			ProductID = cmd.ProductID,
		};
		await ctx.ProductImages.AddAsync(img);
		var res = await ctx.SaveChangesAsync(default);
		if (res == 0)
			return new UnknownError("Failed to add image.");
		return imageMapper.ToProductImageResponse(img);
	}

	public async Task<Result<string>> DeleteImageAsync(int productId, int imageId)
	{
		var img = await ctx.ProductImages.FirstOrDefaultAsync(i =>
			i.ID == imageId && i.ProductID == productId
		);
		if (img == null)
			return new NotFoundError("Image not found.");
		var key = img.Key;
		ctx.ProductImages.Remove(img);
		var res = await ctx.SaveChangesAsync(default);
		if (res == 0)
			return new UnknownError("Failed to delete image.");
		return Result<string>.Ok(key);
	}

	public async Task<Result<DetailedProductResponse>> UpdateAsync(int id, UpdateProductCommand cmd)
	{
		var product = await ctx
			.Products.Include(p => p.Categories)
			.Include(p => p.Images)
			.FirstOrDefaultAsync(p => p.ID == id);
		if (product == null)
		{
			return new NotFoundError("Product not found.");
		}
		var validationResult = await updateProductValidator.ValidateAsync(cmd);
		if (!validationResult.IsValid)
		{
			return new ValidationError(validationResult.ToDictionary());
		}

		mapper.PatchProduct(cmd, product);

		var result = await ctx.SaveChangesAsync(default);
		if (result == 0)
		{
			return new UnknownError("Failed to update product.");
		}
		return mapper.ToDetailedProductResponse(product);
	}
}
