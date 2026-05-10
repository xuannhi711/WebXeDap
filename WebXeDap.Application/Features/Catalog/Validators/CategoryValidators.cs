using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.DTOs;
using WebXeDap.Application.Extensions.Queries;

namespace WebXeDap.Application.Features.Catalog.Validators;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryRequest>
{
	private readonly IApplicationDbContext _ctx;

	public CreateCategoryValidator(IApplicationDbContext ctx)
	{
		_ctx = ctx;

		// RuleLevelCascadeMode = CascadeMode.Stop;

		RuleFor(createReq => createReq.Name)
			.Cascade(CascadeMode.Stop)
			.NotEmpty()
			.WithMessage("Category name is required.");

		RuleFor(createReq => createReq.ParentCategoryID)
			.MustAsync(
				async (parentCategoryID, ct) =>
				{
					return await _ctx.Categories.ByID(parentCategoryID!.Value).AnyAsync(ct);
				}
			)
			.When(createReq => createReq.ParentCategoryID.HasValue)
			.WithMessage("Parent category does not exist.");
	}
}

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryRequest>
{
	private readonly IApplicationDbContext _ctx;

	public UpdateCategoryValidator(IApplicationDbContext ctx)
	{
		_ctx = ctx;

		// RuleLevelCascadeMode = CascadeMode.Stop;

		RuleFor(updateReq => updateReq.ID)
			.Cascade(CascadeMode.Stop)
			.MustAsync(async (id, ct) => await _ctx.Categories.AnyAsync(c => c.ID == id, ct))
			.WithMessage("Category does not exist.");

		RuleFor(updateReq => updateReq.Name)
			.Cascade(CascadeMode.Stop)
			.NotEmpty()
			.WithMessage("Category name is required.");

		RuleFor(updateReq => updateReq.ParentCategoryID)
			.Cascade(CascadeMode.Stop)
			.NotEqual(updateReq => updateReq.ID)
			.WithMessage("A category cannot be its own parent.")
			.MustAsync(
				async (req, parentCategoryID, ct) =>
				{
					return await _ctx.Categories.ByID(parentCategoryID!.Value).AnyAsync(ct);
				}
			)
			.When(updateReq => updateReq.ParentCategoryID.HasValue)
			.WithMessage("Parent category does not exist or is invalid.");
	}
}

public class DeleteCategoryValidator : AbstractValidator<int>
{
	private readonly IApplicationDbContext _ctx;

	public DeleteCategoryValidator(IApplicationDbContext ctx)
	{
		_ctx = ctx;
		RuleFor(id => id)
			.MustAsync(async (id, ct) => await _ctx.Categories.ByID(id).AnyAsync(ct))
			.WithMessage("Category does not exist.");
	}
}
