using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Features.Statistics.DTOs;
using WebXeDap.Domain.Constants;

namespace WebXeDap.WebAPI.Controllers;

[ApiController]
[Route("api/statistics")]
[Authorize(Roles = ROLES.ADMIN)]
public sealed class StatisticsController : ControllerBase
{
	private readonly IStatisticsService statisticsService;

	public StatisticsController(IStatisticsService statisticsService)
	{
		this.statisticsService = statisticsService;
	}

	[HttpGet("overview")]
	public async Task<ActionResult<StatisticsOverviewResponse>> Overview(
		[FromQuery] DateTime? since = null
	)
	{
		var res = await statisticsService.GetOverviewAsync(since);
		return Ok(res);
	}

	[HttpGet("top-products")]
	public async Task<ActionResult<List<TopProductResponse>>> TopProducts(
		[FromQuery] int limit = 10
	)
	{
		var res = await statisticsService.GetTopProductsAsync(limit);
		return Ok(res);
	}
}
