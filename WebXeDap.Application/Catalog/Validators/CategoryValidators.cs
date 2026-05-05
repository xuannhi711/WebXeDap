using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebXeDap.Application.Catalog.DTOs;
using WebXeDap.Application.Common.Interfaces;

namespace WebXeDap.Application.Catalog.Validators;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryValidator(IApplicationDbContext ctx)
    {
        // RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(createReq => createReq.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Category name is required.");

        RuleFor(createReq => createReq.ParentCategoryID)
            .MustAsync(async (parentCategoryID, cancellationToken) =>
            {
                if (parentCategoryID == null)
                {
                    return true;
                }
                return await ctx.Categories.AnyAsync(c => c.ID == parentCategoryID.Value, cancellationToken);
            })
            .WithMessage("Parent category does not exist.");
    }
}

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryValidator(IApplicationDbContext ctx)
    {
        // RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(updateReq => updateReq.ID)
            .Cascade(CascadeMode.Stop)
            .MustAsync(async (id, cancellationToken) =>
                await ctx.Categories.AnyAsync(c => c.ID == id, cancellationToken))
            .WithMessage("Category does not exist.");

        RuleFor(updateReq => updateReq.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Category name is required.");

        RuleFor(updateReq => updateReq.ParentCategoryID)
            .Cascade(CascadeMode.Stop)
            .NotEqual(updateReq => updateReq.ID)
            .WithMessage("A category cannot be its own parent.")
            .MustAsync(async (req, parentCategoryID, cancellationToken) =>
            {
                if (parentCategoryID == null)
                {
                    return true;
                }
                return await ctx.Categories.AnyAsync(c => c.ID == parentCategoryID.Value, cancellationToken);
            })
            .WithMessage("Parent category does not exist or is invalid.");
    }
}

public class DeleteCategoryValidator : AbstractValidator<DeleteCategoryRequest>
{
    public DeleteCategoryValidator(IApplicationDbContext ctx)
    {
        RuleFor(req => req.ID)
            .MustAsync(async (id, cancellationToken) =>
                await ctx.Categories.AnyAsync(c => c.ID == id, cancellationToken))
            .WithMessage("Category does not exist.");
    }
}