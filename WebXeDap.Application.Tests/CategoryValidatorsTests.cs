using WebXeDap.Application.Catalog.Validators;
using WebXeDap.Application.Catalog.DTOs;
using WebXeDap.Application.Catalog;

namespace WebXeDap.Application.Tests;

public sealed class CategoryValidatorsTests
{
    [Fact]
    public async Task CreateCategoryValidator_Fail_When_Name_Is_Empty()
    {
        using var ctx = ServiceTestHelpers.CreateContext();
        var validator = new CreateCategoryValidator(ctx);

        var result = await validator.ValidateAsync(new CreateCategoryRequest(
            Name: "",
            ParentCategoryID: 1
        ));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateCategoryRequest.Name));
    }

    [Fact]
    public async Task CreateCategoryValidator_Fail_When_ParentCategoryID_Does_Not_Exist()
    {
        using var ctx = ServiceTestHelpers.CreateContext();
        var validator = new CreateCategoryValidator(ctx);

        var result = await validator.ValidateAsync(new CreateCategoryRequest(
            Name: "New Category",
            ParentCategoryID: 999
        ));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateCategoryRequest.ParentCategoryID));
    }

    [Fact]
    public async Task CreateCategoryValidator_Pass_When_Name_Is_Valid()
    {
        using var ctx = ServiceTestHelpers.CreateContext();
        var validator = new CreateCategoryValidator(ctx);

        var result = await validator.ValidateAsync(new CreateCategoryRequest(
            Name: "New Category",
            ParentCategoryID: null
        ));

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task CreateCategoryValidator_Pass_When_ParentCategoryID_Is_Valid()
    {
        using var ctx = ServiceTestHelpers.CreateContext();
        var currentUser = new TestCurrentUserService { UserId = 1 };
        var service = new CatalogService(ctx, currentUser);
        var validator = new CreateCategoryValidator(ctx);

        var parentCategoryId = await service.CreateCategoryAsync(
            new CreateCategoryRequest(
                Name: "Parent Category",
                ParentCategoryID: null
            ),
            cancellationToken: CancellationToken.None
        );

        var result = await validator.ValidateAsync(new CreateCategoryRequest(
            Name: "New Category",
            ParentCategoryID: parentCategoryId
        ));

        Assert.True(result.IsValid);
    }
}

public sealed class UpdateCategoryValidatorTests
{
    // Similar tests for UpdateCategoryValidator can be implemented here
}