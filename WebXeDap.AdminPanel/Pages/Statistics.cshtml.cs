using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Features.Statistics.DTOs;

namespace WebXeDap.AdminPanel.Pages;

public sealed class StatisticsModel : PageModel
{
	private readonly IStatisticsService statisticsService;

	public StatisticsModel(IStatisticsService statisticsService)
	{
		this.statisticsService = statisticsService;
	}

	[BindProperty(SupportsGet = true)]
	public int? Days { get; set; }

	[BindProperty(SupportsGet = true)]
	public int Top { get; set; } = 10;

	public StatisticsOverviewResponse? Overview { get; private set; }

	public IReadOnlyList<TopProductResponse> TopProducts { get; private set; } = [];

	public async Task OnGetAsync()
	{
		if (Top <= 0)
		{
			Top = 10;
		}
		else if (Top > 50)
		{
			Top = 50;
		}

		DateTime? since = null;
		if (Days.HasValue && Days.Value > 0)
		{
			since = DateTime.UtcNow.AddDays(-Days.Value);
		}

		Overview = await statisticsService.GetOverviewAsync(since);
		TopProducts = await statisticsService.GetTopProductsAsync(Top);
	}
}
