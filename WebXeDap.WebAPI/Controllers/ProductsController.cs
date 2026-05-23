using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
	private readonly IFileStorage fileStorage;

	public ProductsController(IProductService productService, IFileStorage fileStorage)
	{
		this.productService = productService;
		this.fileStorage = fileStorage;
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

	[HttpGet("{id:int}/images")]
	public async Task<ActionResult<List<ProductImageResponse>>> ListImages(int id)
	{
		var images = await productService.ListImagesAsync(id);
		return Ok(images);
	}

	[HttpPost("{id:int}/images")]
	[Authorize(Roles = ROLES.ADMIN)]
	public async Task<IActionResult> UploadImage(int id, IFormFile file, [FromForm] int? order)
	{
		if (file == null || file.Length == 0)
			return BadRequest("File is required.");
		using var stream = file.OpenReadStream();
		var key = await fileStorage.SaveFileAsync(stream, file.FileName, file.ContentType);
		var result = await productService.AddImageAsync(
			new CreateProductImageCommand(id, key, order ?? 0)
		);
		return result.Match(
			val => CreatedAtAction(nameof(ListImages), new { id }, val),
			err => this.MatchErrorResult(err)
		);
	}

	// [HttpDelete("{id:int}/images/{imageId:int}")]
	// [Authorize(Roles = ROLES.ADMIN)]
	// public async Task<IActionResult> DeleteImage(int id, int imageId)
	// {
	// 	var res = await productService.DeleteImageAsync(id, imageId);
	// 	return await res.MatchAsync(
	// 		async key =>
	// 		{
	// 			// attempt to delete file from storage
	// 			try
	// 			{
	// 				await fileStorage.DeleteFileAsync(key);
	// 			}
	// 			catch
	// 			{
	// 				// swallow storage deletion errors to avoid failing the request
	// 			}
	// 			return NoContent();
	// 		},
	// 		err => Task.FromResult(this.MatchErrorResult(err))
	// 	);
	// }

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
