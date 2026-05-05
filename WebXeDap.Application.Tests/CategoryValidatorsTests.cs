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
    [Fact]
    public async Task UpdateCategoryValidator_Fail_When_ID_Does_Not_Exist()
    {
        using var ctx = ServiceTestHelpers.CreateContext();
        var validator = new UpdateCategoryValidator(ctx);

        var result = await validator.ValidateAsync(new UpdateCategoryRequest(
            ID: 999,
            Name: "Updated Category",
            ParentCategoryID: null
        ));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateCategoryRequest.ID));
    }

    [Fact]
    public async Task UpdateCategoryValidator_Fail_When_Name_Is_Empty()
    {
        using var ctx = ServiceTestHelpers.CreateContext();
        var validator = new UpdateCategoryValidator(ctx);
        var currentUser = new TestCurrentUserService { UserId = 1 };
        var service = new CatalogService(ctx, currentUser);

        var parentCategoryId = await service.CreateCategoryAsync(
            new CreateCategoryRequest(
                Name: "New Category",
                ParentCategoryID: null
            ),
            cancellationToken: CancellationToken.None
        );

        var result = await validator.ValidateAsync(new UpdateCategoryRequest(
            ID: parentCategoryId,
            Name: "",
            ParentCategoryID: null
        ));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Category name is required."));
    }

    [Fact]
    public async Task UpdateCategoryValidator_Fail_When_ParentCategoryID_Is_Same_As_ID()
    {
        using var ctx = ServiceTestHelpers.CreateContext();
        var validator = new UpdateCategoryValidator(ctx);
        var currentUser = new TestCurrentUserService { UserId = 1 };
        var service = new CatalogService(ctx, currentUser);

        var categoryId = await service.CreateCategoryAsync(
            new CreateCategoryRequest(
                Name: "New Category",
                ParentCategoryID: null
            ),
            cancellationToken: CancellationToken.None
        );

        var result = await validator.ValidateAsync(new UpdateCategoryRequest(
            ID: categoryId,
            Name: "Updated Category",
            ParentCategoryID: categoryId
        ));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("A category cannot be its own parent."));
    }

    [Fact]
    public async Task UpdateCategoryValidator_Fail_When_ParentCategoryID_Is_Invalid()
    {
        using var ctx = ServiceTestHelpers.CreateContext();
        var validator = new UpdateCategoryValidator(ctx);
        var currentUser = new TestCurrentUserService { UserId = 1 };
        var service = new CatalogService(ctx, currentUser);

        var categoryId = await service.CreateCategoryAsync(
            new CreateCategoryRequest(
                Name: "New Category",
                ParentCategoryID: null
            ),
            cancellationToken: CancellationToken.None
        );

        var result = await validator.ValidateAsync(new UpdateCategoryRequest(
            ID: categoryId,
            Name: "Updated Category",
            ParentCategoryID: 999
        ));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Parent category does not exist or is invalid."));
    }
}

public sealed class DeleteCategoryValidatorTests
{
    [Fact]
    public async Task DeleteCategoryValidator_Fail_When_ID_Does_Not_Exist()
    {
        using var ctx = ServiceTestHelpers.CreateContext();
        var validator = new DeleteCategoryValidator(ctx);

        var result = await validator.ValidateAsync(new DeleteCategoryRequest(ID: 999));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(DeleteCategoryRequest.ID));
    }

    [Fact]
    public async Task DeleteCategoryValidator_Pass_When_ID_Exists()
    {
        using var ctx = ServiceTestHelpers.CreateContext();
        var validator = new DeleteCategoryValidator(ctx);
        var currentUser = new TestCurrentUserService { UserId = 1 };
        var service = new CatalogService(ctx, currentUser);

        var categoryId = await service.CreateCategoryAsync(
            new CreateCategoryRequest(
                Name: "New Category",
                ParentCategoryID: null
            ),
            cancellationToken: CancellationToken.None
        );

        var result = await validator.ValidateAsync(new DeleteCategoryRequest(ID: categoryId));
        Assert.True(result.IsValid);
    }
}