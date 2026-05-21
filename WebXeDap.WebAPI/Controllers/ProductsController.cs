using Microsoft.AspNetCore.Mvc;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.WebAPI.Extensions;

namespace WebXeDap.WebAPI.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
	private readonly IProductService productService;

	public ProductsController(IProductService productService)
	{
		this.productService = productService;
	}

	[HttpGet]
	public async Task<ActionResult<List<SimpleProductResponse>>> List(
		[FromQuery] FilterProductCommand filter,
		[FromQuery] int page = 1,
		[FromQuery] int size = 20
	)
	{
		var products = await productService.FilterAsync(filter, page, size);
		return Ok(products);
	}

	[HttpGet("{id:int}")]
	public async Task<ActionResult<DetailedProductResponse>> GetByID(int id)
	{
		var result = await productService.GetByIDAsync(id);
		return result.Match(Ok, this.MatchErrorResult);
	}

	[HttpGet("count")]
	public async Task<ActionResult<int>> Count([FromQuery] FilterProductCommand filter)
	{
		var total = await productService.CountAsync(filter);
		return Ok(total);
	}

	[HttpPost]
	public async Task<ActionResult<SimpleProductResponse>> Create(
		[FromBody] CreateProductRequest request
	)
	{
		var result = await productService.CreateAsync(
			new CreateProductCommand(
				request.Name,
				request.Description,
				request.Price,
				request.Quantity,
				request.CategoryIDs,
				request.CurrencySymbol
			)
		);
		return result.Match(val => Created(nameof(GetByID), val), this.MatchErrorResult);
	}

	[HttpPut("{id:int}")]
	public async Task<ActionResult<DetailedProductResponse>> Update(
		int id,
		[FromBody] UpdateProductRequest request
	)
	{
		var result = await productService.UpdateAsync(
			new UpdateProductCommand(
				id,
				request.Name,
				request.Description,
				request.Price,
				request.Quantity,
				request.CategoryIDs,
				request.CurrencySymbol
			)
		);
		return result.Match(Ok, this.MatchErrorResult);
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> Delete(int id)
	{
		var result = await productService.DeleteAsync(id);
		return result.Match(_ => NoContent(), this.MatchErrorResult);
	}

	public sealed record CreateProductRequest(
		string Name,
		string? Description,
		decimal Price,
		int Quantity,
		int[]? CategoryIDs,
		string CurrencySymbol = "VNĐ"
	);

	public sealed record UpdateProductRequest(
		string Name,
		string? Description,
		decimal Price,
		int Quantity,
		int[]? CategoryIDs,
		string CurrencySymbol = "VNĐ"
	);
}
