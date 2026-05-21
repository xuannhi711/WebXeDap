using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Util.Primitives.ResultType;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Extensions;
using WebXeDap.Application.Features.Sales.DTOs;
using WebXeDap.Application.Features.Sales.Mapper;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Features.Sales;

public class SaleCampaignService : ISaleCampaignService
{
	private readonly IApplicationDbContext ctx;
	private readonly SaleCampaignMapper mapper;
	private readonly IValidator<CreateSaleCampaignCommand> createValidator;
	private readonly IValidator<UpdateSaleCampaignCommand> updateValidator;

	public SaleCampaignService(
		IApplicationDbContext ctx,
		SaleCampaignMapper mapper,
		IValidator<CreateSaleCampaignCommand> createValidator,
		IValidator<UpdateSaleCampaignCommand> updateValidator
	)
	{
		this.ctx = ctx;
		this.mapper = mapper;
		this.createValidator = createValidator;
		this.updateValidator = updateValidator;
	}

	public async Task<List<SaleCampaignResponse>> ListAsync(bool activeOnly)
	{
		var now = DateTime.UtcNow;
		IQueryable<SaleCampaign> query = ctx
			.SaleCampaigns.AsNoTracking()
			.Include(s => s.Products);

		if (activeOnly)
		{
			query = query.Where(s => s.IsActive(now));
		}

		var campaigns = await query.OrderByDescending(s => s.StartsAt).ToListAsync(default);
		return [.. campaigns.Select(mapper.ToSaleCampaignResponse)];
	}

	public async Task<Result<SaleCampaignResponse>> GetByIDAsync(int id)
	{
		var campaign = await ctx
			.SaleCampaigns.AsNoTracking()
			.Include(s => s.Products)
			.FirstOrDefaultAsync(s => s.ID == id, default);

		if (campaign is null)
		{
			return new NotFoundError("Sale campaign not found.");
		}

		return mapper.ToSaleCampaignResponse(campaign);
	}

	public async Task<Result<SaleCampaignResponse>> CreateAsync(CreateSaleCampaignCommand cmd)
	{
		var validationResult = await createValidator.ValidateAsync(cmd);
		if (!validationResult.IsValid)
		{
			return validationResult.ToValidationError();
		}

		var campaign = mapper.ToSaleCampaign(cmd);
		await ctx.SaleCampaigns.AddAsync(campaign, default);
		var res = await ctx.SaveChangesAsync(default);
		if (res == 0)
		{
			return new UnknownError("Failed to create sale campaign.");
		}

		return mapper.ToSaleCampaignResponse(campaign);
	}

	public async Task<Result<SaleCampaignResponse>> UpdateAsync(
		int id,
		UpdateSaleCampaignCommand cmd
	)
	{
		var campaign = await ctx
			.SaleCampaigns.Include(s => s.Products)
			.FirstOrDefaultAsync(s => s.ID == id, default);

		if (campaign is null)
		{
			return new NotFoundError("Sale campaign not found.");
		}

		var validationResult = await updateValidator.ValidateAsync(cmd);
		if (!validationResult.IsValid)
		{
			return validationResult.ToValidationError();
		}

		// TODO: validate the combination of StartsAt and EndsAt if either of them is being updated

		mapper.PatchSaleCampaign(cmd, campaign);
		var res = await ctx.SaveChangesAsync(default);
		if (res == 0)
		{
			return new UnknownError("Failed to update sale campaign.");
		}

		return mapper.ToSaleCampaignResponse(campaign);
	}

	public async Task<Result> DeleteAsync(int id)
	{
		var campaign = await ctx.SaleCampaigns.FindAsync(id);
		if (campaign is null)
		{
			return new NotFoundError("Sale campaign not found.");
		}

		ctx.SaleCampaigns.Remove(campaign);
		var res = await ctx.SaveChangesAsync(default);
		if (res == 0)
		{
			return new UnknownError("Failed to delete sale campaign.");
		}
		return Result.Ok();
	}
}
