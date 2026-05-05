using FluentValidation;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.DTOs;
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
				async (parentCategoryID, ct) =>
				{
					var spec = new CategoryByIDSpec(parentCategoryID!.Value);
					return await _categoryRepo.AnyAsync(spec, ct);
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
			.MustAsync(async (id, ct) => await _categoryRepo.AnyAsync(new CategoryByIDSpec(id), ct))
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
					var spec = new CategoryByIDSpec(parentCategoryID!.Value);
					return await _categoryRepo.AnyAsync(spec, ct);
				}
			)
			.When(updateReq => updateReq.ParentCategoryID != null)
			.WithMessage("Parent category does not exist or is invalid.");
	}
}

public class DeleteCategoryValidator : AbstractValidator<int>
{
	private readonly ICategoryRepository _categoryRepo;

	public DeleteCategoryValidator(ICategoryRepository categoryRepo)
	{
		_categoryRepo = categoryRepo;
		RuleFor(id => id)
			.MustAsync(async (id, ct) => await _categoryRepo.AnyAsync(new CategoryByIDSpec(id), ct))
			.WithMessage("Category does not exist.");
	}
}
