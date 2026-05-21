using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Features.Sales.DTOs;
using WebXeDap.Domain.Enums;

namespace WebXeDap.Application.Features.Sales.Validators;

public sealed class CreateSaleCampaignValidator : AbstractValidator<CreateSaleCampaignCommand>
{
	public CreateSaleCampaignValidator(IApplicationDbContext ctx)
	{
		RuleFor(cmd => cmd.Name)
			.Cascade(CascadeMode.Stop)
			.NotEmpty()
			.WithMessage("Sale campaign name is required.");

		RuleFor(cmd => cmd.DiscountType)
			.IsInEnum()
			.WithMessage("Discount type is invalid.");

		RuleFor(cmd => cmd.DiscountValue)
			.Cascade(CascadeMode.Stop)
			.GreaterThan(0)
			.WithMessage("Discount value must be greater than 0.");

		RuleFor(cmd => cmd.DiscountValue)
			.Cascade(CascadeMode.Stop)
			.LessThanOrEqualTo(100)
			.When(cmd => cmd.DiscountType == DiscountType.Percentage)
			.WithMessage("Percentage discount must be <= 100.");

		RuleFor(cmd => cmd.StartsAt)
			.LessThan(cmd => cmd.EndsAt)
			.WithMessage("EndsAt must be after StartsAt.");

		RuleFor(cmd => cmd.ProductIDs)
			.Cascade(CascadeMode.Stop)
			.NotEmpty()
			.WithMessage("At least one product is required.")
			.MustAsync(
				async (productIDs, ct) =>
					await ctx.Products.CountAsync(p => productIDs.Contains(p.ID), ct)
					== productIDs.Count
			)
			.WithMessage("One or more selected products are invalid.");
	}
}

public sealed class UpdateSaleCampaignValidator : AbstractValidator<UpdateSaleCampaignCommand>
{
	public UpdateSaleCampaignValidator(IApplicationDbContext ctx)
	{
		RuleFor(cmd => cmd.Name)
			.Cascade(CascadeMode.Stop)
			.NotEmpty()
			.When(cmd => cmd.Name is not null)
			.WithMessage("Sale campaign name is required.");

		RuleFor(cmd => cmd.DiscountType)
			.IsInEnum()
			.When(cmd => cmd.DiscountType.HasValue)
			.WithMessage("Discount type is invalid.");

		RuleFor(cmd => cmd.DiscountValue)
			.Cascade(CascadeMode.Stop)
			.GreaterThan(0)
			.When(cmd => cmd.DiscountValue.HasValue)
			.WithMessage("Discount value must be greater than 0.");

		RuleFor(cmd => cmd.DiscountValue)
			.Cascade(CascadeMode.Stop)
			.LessThanOrEqualTo(100)
			.When(cmd =>
				cmd.DiscountValue.HasValue && cmd.DiscountType == DiscountType.Percentage
			)
			.WithMessage("Percentage discount must be <= 100.");

		RuleFor(cmd => cmd.StartsAt)
			.LessThan(cmd => cmd.EndsAt)
			.When(cmd => cmd.StartsAt.HasValue && cmd.EndsAt.HasValue)
			.WithMessage("EndsAt must be after StartsAt.");

		RuleFor(cmd => cmd.ProductIDs)
			.Cascade(CascadeMode.Stop)
			.MustAsync(
				async (productIDs, ct) =>
				{
					if (productIDs == null)
					{
						return true;
					}
					return await ctx.Products.CountAsync(p => productIDs.Contains(p.ID), ct)
						== productIDs.Count;
				}
			)
			.WithMessage("One or more selected products are invalid.");
	}
}
