using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.Application.Features.Catalog.Queries;

namespace WebXeDap.Application.Features.Catalog.Validators;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
	public CreateCategoryValidator(IApplicationDbContext ctx)
	{
		// RuleLevelCascadeMode = CascadeMode.Stop;

		RuleFor(createReq => createReq.Name)
			.Cascade(CascadeMode.Stop)
			.NotEmpty()
			.WithMessage("Category name is required.");

		RuleFor(createReq => createReq.ParentCategoryID)
			.MustAsync(
				async (parentCategoryID, ct) =>
				{
					return await ctx.Categories.ByID(parentCategoryID!.Value).AnyAsync(ct);
				}
			)
			.When(createReq => createReq.ParentCategoryID.HasValue)
			.WithMessage("Parent category does not exist.");
	}
}

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
	public UpdateCategoryValidator(IApplicationDbContext ctx)
	{
		// RuleLevelCascadeMode = CascadeMode.Stop;

		RuleFor(updateReq => updateReq.Name)
			.Cascade(CascadeMode.Stop)
			.NotEmpty()
			.WithMessage("Category name is required.");

		RuleFor(updateReq => updateReq.ParentCategoryID)
			.Cascade(CascadeMode.Stop)
			// .NotEqual(updateReq => updateReq.ID)
			// .WithMessage("A category cannot be its own parent.")
			.MustAsync(
				async (req, parentCategoryID, ct) =>
				{
					return await ctx.Categories.ByID(parentCategoryID!.Value).AnyAsync(ct);
				}
			)
			.When(updateReq => updateReq.ParentCategoryID.HasValue)
			.WithMessage("Parent category does not exist or is invalid.");
	}
}
