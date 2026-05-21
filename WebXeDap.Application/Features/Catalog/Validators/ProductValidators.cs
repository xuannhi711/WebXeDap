using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.Application.Features.Catalog.Queries;

namespace WebXeDap.Application.Features.Catalog.Validators;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
	private readonly IApplicationDbContext _ctx;

	public CreateProductValidator(IApplicationDbContext ctx)
	{
		_ctx = ctx;

		// RuleLevelCascadeMode = CascadeMode.Stop;

		RuleFor(createReq => createReq.Name)
			.Cascade(CascadeMode.Stop)
			.NotEmpty()
			.WithMessage("Product name is required.");

		RuleFor(createReq => createReq.Price)
			.Cascade(CascadeMode.Stop)
			.GreaterThanOrEqualTo(0)
			.WithMessage("Product price must >= 0");

		RuleFor(createReq => createReq.Quantity)
			.Cascade(CascadeMode.Stop)
			.GreaterThanOrEqualTo(0)
			.WithMessage("Product quantity must >= 0");

		RuleFor(createReq => createReq.CategoryIDs)
			.Cascade(CascadeMode.Stop)
			.MustAsync(
				async (categoryIDs, ct) =>
				{
					if (categoryIDs == null || categoryIDs.Count == 0)
						return true;
					return await _ctx
							.Categories.Where(c => categoryIDs.Contains(c.ID))
							.CountAsync(ct) == categoryIDs.Count;
				}
			)
			.WithMessage("One or more selected categories are invalid.");
	}
}

public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
	private readonly IApplicationDbContext _ctx;

	public UpdateProductValidator(IApplicationDbContext ctx)
	{
		_ctx = ctx;

		RuleFor(updateReq => updateReq.Name)
			.Cascade(CascadeMode.Stop)
			.NotEmpty()
			.WithMessage("Product name is required.");

		RuleFor(updateReq => updateReq.Price)
			.Cascade(CascadeMode.Stop)
			.GreaterThanOrEqualTo(0)
			.WithMessage("Product price must >= 0");

		RuleFor(updateReq => updateReq.Quantity)
			.Cascade(CascadeMode.Stop)
			.GreaterThanOrEqualTo(0)
			.WithMessage("Product quantity must >= 0");

		RuleFor(updateReq => updateReq.CategoryIDs)
			.Cascade(CascadeMode.Stop)
			.MustAsync(
				async (categoryIDs, ct) =>
				{
					if (categoryIDs == null || categoryIDs.Count == 0)
						return true;
					return await _ctx
							.Categories.Where(c => categoryIDs.Contains(c.ID))
							.CountAsync(ct) == categoryIDs.Count;
				}
			)
			.WithMessage("One or more selected categories are invalid.");
	}
}
