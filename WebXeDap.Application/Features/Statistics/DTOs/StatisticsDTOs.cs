namespace WebXeDap.Application.Features.Statistics.DTOs;

public sealed record StatisticsOverviewResponse(
	int TotalUsers,
	int TotalProducts,
	int TotalOrders,
	decimal TotalRevenue,
	int ActiveSaleCampaigns
);

public sealed record TopProductResponse(
	int ProductID,
	string ProductName,
	int QuantitySold,
	decimal Revenue
);
