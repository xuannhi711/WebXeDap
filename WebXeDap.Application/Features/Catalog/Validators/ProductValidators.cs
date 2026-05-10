using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.DTOs;
using WebXeDap.Application.Features.Catalog.Queries;

namespace WebXeDap.Application.Features.Catalog.Validators;

public class CreateProductValidator : AbstractValidator<CreateProductRequest>
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
	}
}

public class UpdateProductValidator : AbstractValidator<UpdateProductRequest>
{
	private readonly IApplicationDbContext _ctx;

	public UpdateProductValidator(IApplicationDbContext ctx)
	{
		_ctx = ctx;

		// RuleLevelCascadeMode = CascadeMode.Stop;

		RuleFor(updateReq => updateReq.ID)
			.Cascade(CascadeMode.Stop)
			.MustAsync(async (id, ct) => await _ctx.Products.ByID(id).AnyAsync(ct))
			.WithMessage("Product does not exist.");

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
	}
}

public class DeleteProductValidator : AbstractValidator<int>
{
	private readonly IApplicationDbContext _ctx;

	public DeleteProductValidator(IApplicationDbContext ctx)
	{
		_ctx = ctx;
		RuleFor(id => id)
			.MustAsync(async (id, ct) => await _ctx.Products.ByID(id).AnyAsync(ct))
			.WithMessage("Product does not exist.");
	}
}
