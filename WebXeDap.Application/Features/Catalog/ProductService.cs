using Microsoft.EntityFrameworkCore;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.DTOs;
using WebXeDap.Application.Features.Catalog.Mapper;
using WebXeDap.Application.Features.Catalog.Queries;
using WebXeDap.Application.QueryExtensions;

namespace WebXeDap.Application.Features.Catalog;

public class ProductService : IProductService
{
	private readonly IApplicationDbContext _ctx;
	private readonly ProductMapper _mapper;

	public ProductService(IApplicationDbContext ctx, ProductMapper mapper)
	{
		_ctx = ctx;
		_mapper = mapper;
	}

	public async Task<SimpleProductResponse?> CreateAsync(CreateProductRequest req)
	{
		var product = _mapper.ToProduct(req);
		await _ctx.Products.AddAsync(product);
		var res = await _ctx.SaveChangesAsync(default);
		if (res == 0)
		{
			return null;
		}
		return _mapper.ToSimpleProductResponse(product);
	}

	public async Task<bool> DeleteAsync(int id)
	{
		var product = await _ctx.Products.ByID(id).FirstOrDefaultAsync(default);
		if (product == null)
		{
			return false;
		}
		_ctx.Products.Remove(product);
		var res = await _ctx.SaveChangesAsync(default);
		return res == 1;
	}

	public async Task<PaginatedResult<SimpleProductResponse>> FilterAsync(FilterProductRequest req)
	{
		var products = await _ctx
			.Products.Filter(req)
			.ApplySorting(req)
			.Paginate(req.Page, req.Size)
			.ToListAsync(default);
		var total = await _ctx.Products.Filter(req).CountAsync(default);
		return new PaginatedResult<SimpleProductResponse>(
			Items: [.. products.Select(_mapper.ToSimpleProductResponse)],
			Total: total,
			Page: req.Page,
			Size: products.Count
		);
	}

	public async Task<DetailedProductResponse?> GetByIDAsync(int id)
	{
		var product = await _ctx.Products.ByID(id).FirstOrDefaultAsync(default);
		if (product == null)
		{
			return null;
		}
		return _mapper.ToDetailedProductResponse(product);
	}

	public async Task<DetailedProductResponse?> UpdateAsync(UpdateProductRequest req)
	{
		var product = _mapper.ToProduct(req);
		_ctx.Products.Update(product);
		var result = await _ctx.SaveChangesAsync(default);
		if (result == 0)
		{
			return null;
		}
		return _mapper.ToDetailedProductResponse(product);
	}
}
