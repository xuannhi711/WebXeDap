using WebXeDap.Application.Features.Statistics.DTOs;

namespace WebXeDap.Application.Contracts.Services;

public interface IStatisticsService
{
	Task<StatisticsOverviewResponse> GetOverviewAsync(DateTime? since = null);

	Task<List<TopProductResponse>> GetTopProductsAsync(int limit = 10);
}
