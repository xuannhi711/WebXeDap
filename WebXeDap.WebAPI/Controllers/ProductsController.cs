using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.Domain.Constants;
using WebXeDap.WebAPI.Exchange;
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
	public async Task<ActionResult<PagedProductResponse>> List(
		[FromQuery] FilterProductCommand filter,
		[FromQuery] int page = 1,
		[FromQuery] int size = 20
	)
	{
		var products = await productService.FilterAsync(filter, page, size);
		var total = await productService.CountAsync(filter);
		return Ok(new PagedProductResponse(total, page, size, products));
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
	[Authorize(Roles = ROLES.ADMIN)]
	public async Task<ActionResult<SimpleProductResponse>> Create(
		[FromBody] CreateProductCommand cmd
	)
	{
		var result = await productService.CreateAsync(cmd);
		return result.Match(val => Created(nameof(GetByID), val), this.MatchErrorResult);
	}

	[HttpPut("{id:int}")]
	[Authorize(Roles = ROLES.ADMIN)]
	public async Task<ActionResult<DetailedProductResponse>> Update(
		int id,
		[FromBody] UpdateProductCommand cmd
	)
	{
		var result = await productService.UpdateAsync(id, cmd);
		return result.Match(Ok, this.MatchErrorResult);
	}

	[HttpDelete("{id:int}")]
	[Authorize(Roles = ROLES.ADMIN)]
	public async Task<IActionResult> Delete(int id)
	{
		var result = await productService.DeleteAsync(id);
		return result.Match(_ => NoContent(), this.MatchErrorResult);
	}
}
