using Util.Primitives.ResultType;
using WebXeDap.Application.Features.Sales.DTOs;

namespace WebXeDap.Application.Contracts.Services;

public interface ISaleCampaignService
{
	Task<List<SaleCampaignResponse>> ListAsync(bool activeOnly);

	Task<Result<SaleCampaignResponse>> GetByIDAsync(int id);

	Task<Result<SaleCampaignResponse>> CreateAsync(CreateSaleCampaignCommand cmd);

	Task<Result<SaleCampaignResponse>> UpdateAsync(int id, UpdateSaleCampaignCommand cmd);

	Task<Result> DeleteAsync(int id);
}
