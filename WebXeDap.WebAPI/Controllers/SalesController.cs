using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Features.Sales.DTOs;
using WebXeDap.Domain.Constants;
using WebXeDap.WebAPI.Extensions;

namespace WebXeDap.WebAPI.Controllers;

[ApiController]
[Route("api/sales")]
public sealed class SalesController : ControllerBase
{
	private readonly ISaleCampaignService saleService;

	public SalesController(ISaleCampaignService saleService)
	{
		this.saleService = saleService;
	}

	[HttpGet]
	public async Task<ActionResult<List<SaleCampaignResponse>>> List(
		[FromQuery] bool activeOnly = false
	)
	{
		var campaigns = await saleService.ListAsync(activeOnly);
		return Ok(campaigns);
	}

	[HttpGet("{id:int}")]
	public async Task<ActionResult<SaleCampaignResponse>> GetById(int id)
	{
		var result = await saleService.GetByIDAsync(id);
		return result.Match(Ok, this.MatchErrorResult);
	}

	[Authorize(Roles = ROLES.ADMIN)]
	[HttpPost]
	public async Task<ActionResult<SaleCampaignResponse>> Create(
		[FromBody] CreateSaleCampaignCommand cmd
	)
	{
		var result = await saleService.CreateAsync(cmd);
		return result.Match(val => Created(nameof(GetById), val), this.MatchErrorResult);
	}

	[Authorize(Roles = ROLES.ADMIN)]
	[HttpPut("{id:int}")]
	public async Task<ActionResult<SaleCampaignResponse>> Update(
		int id,
		[FromBody] UpdateSaleCampaignCommand cmd
	)
	{
		var result = await saleService.UpdateAsync(id, cmd);
		return result.Match(Ok, this.MatchErrorResult);
	}

	[Authorize(Roles = ROLES.ADMIN)]
	[HttpDelete("{id:int}")]
	public async Task<IActionResult> Delete(int id)
	{
		var result = await saleService.DeleteAsync(id);
		return result.Match(_ => NoContent(), this.MatchErrorResult);
	}
}
