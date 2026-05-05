using FluentValidation;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.Application.Features.Catalog.Specs;

namespace WebXeDap.Application.Features.Catalog.Validators;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryRequest>
{
	private readonly ICategoryRepository _categoryRepo;

	public CreateCategoryValidator(ICategoryRepository categoryRepo)
	{
		_categoryRepo = categoryRepo;

		// RuleLevelCascadeMode = CascadeMode.Stop;

		RuleFor(createReq => createReq.Name)
			.Cascade(CascadeMode.Stop)
			.NotEmpty()
			.WithMessage("Category name is required.");

		RuleFor(createReq => createReq.ParentCategoryID)
			.MustAsync(
				async (parentCategoryID, cancellationToken) =>
				{
					var spec = new CategoryByIDSpec(parentCategoryID!.Value);
					return await _categoryRepo.AnyAsync(spec, cancellationToken);
				}
			)
			.When(createReq => createReq.ParentCategoryID != null)
			.WithMessage("Parent category does not exist.");
	}
}

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryRequest>
{
	private readonly ICategoryRepository _categoryRepo;

	public UpdateCategoryValidator(ICategoryRepository categoryRepo)
	{
		_categoryRepo = categoryRepo;

		// RuleLevelCascadeMode = CascadeMode.Stop;

		RuleFor(updateReq => updateReq.ID)
			.Cascade(CascadeMode.Stop)
			.MustAsync(
				async (id, cancellationToken) =>
					await _categoryRepo.AnyAsync(new CategoryByIDSpec(id), cancellationToken)
			)
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
				async (req, parentCategoryID, cancellationToken) =>
				{
					var spec = new CategoryByIDSpec(parentCategoryID!.Value);
					return await _categoryRepo.AnyAsync(spec, cancellationToken);
				}
			)
			.When(updateReq => updateReq.ParentCategoryID != null)
			.WithMessage("Parent category does not exist or is invalid.");
	}
}

public class DeleteCategoryValidator : AbstractValidator<DeleteCategoryRequest>
{
	private readonly ICategoryRepository _categoryRepo;

	public DeleteCategoryValidator(ICategoryRepository categoryRepo)
	{
		_categoryRepo = categoryRepo;
		RuleFor(req => req.ID)
			.MustAsync(
				async (id, cancellationToken) =>
					await _categoryRepo.AnyAsync(new CategoryByIDSpec(id), cancellationToken)
			)
			.WithMessage("Category does not exist.");
	}
}
