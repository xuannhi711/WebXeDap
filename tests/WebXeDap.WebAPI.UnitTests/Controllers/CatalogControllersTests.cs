using Microsoft.AspNetCore.Mvc;
using Util.Primitives.ResultType;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.WebAPI.Controllers;

namespace WebXeDap.WebAPI.UnitTests.Controllers;

[Trait("Category", "Unit")]
public class CatalogControllersTests
{
	[Fact]
	public async Task Products_GetById_ReturnsNotFound_WhenMissing()
	{
		var controller = new ProductsController(
			new StubProductService { GetByIdResult = new NotFoundError("missing") }
		);

		var result = await controller.GetByID(123);

		Assert.IsType<NotFoundObjectResult>(result.Result);
	}

	[Fact]
	public async Task Products_List_ReturnsOk_WithItems()
	{
		var items = new List<SimpleProductResponse>
		{
			new(1, "Bike", null, 200m, "VND", 5, null, DateTime.UtcNow, null, false, null),
		};
		var controller = new ProductsController(new StubProductService { FilterResult = items });

		var result = await controller.List(new FilterProductCommand(null, null, null, null), 1, 20);

		var ok = Assert.IsType<OkObjectResult>(result.Result);
		var payload = Assert.IsAssignableFrom<List<SimpleProductResponse>>(ok.Value);
		Assert.Single(payload);
	}

	[Fact]
	public async Task Products_Count_ReturnsOk()
	{
		var controller = new ProductsController(new StubProductService { CountResult = 3 });

		var result = await controller.Count(new FilterProductCommand(null, null, null, null));

		var ok = Assert.IsType<OkObjectResult>(result.Result);
		Assert.Equal(3, ok.Value);
	}

	[Fact]
	public async Task Products_Create_ReturnsBadRequest_WhenValidationError()
	{
		var controller = new ProductsController(
			new StubProductService
			{
				CreateResult = new ValidationError(
					new Dictionary<string, string[]> { ["name"] = ["required"] }
				),
			}
		);

		var result = await controller.Create(
			new ProductsController.CreateProductRequest(
				Name: "",
				Description: null,
				Price: 10,
				Quantity: 1,
				CategoryIDs: null
			)
		);

		var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
		Assert.Equal(400, badRequest.StatusCode);
	}

	[Fact]
	public async Task Products_Update_ReturnsOk_WhenSuccess()
	{
		var response = new DetailedProductResponse(
			ID: 1,
			Name: "Bike",
			Description: null,
			Price: 100m,
			Quantity: 5,
			Categories: [],
			Images: [],
			CurrencySymbol: "VND",
			CreatedAt: DateTime.UtcNow,
			UpdatedAt: null,
			IsDeleted: false,
			DeletedAt: null
		);
		var controller = new ProductsController(new StubProductService { UpdateResult = response });

		var result = await controller.Update(
			1,
			new ProductsController.UpdateProductRequest(
				Name: "Bike",
				Description: null,
				Price: 100m,
				Quantity: 5,
				CategoryIDs: null
			)
		);

		var ok = Assert.IsType<OkObjectResult>(result.Result);
		Assert.IsType<DetailedProductResponse>(ok.Value);
	}

	[Fact]
	public async Task Products_Delete_ReturnsNoContent_WhenSuccess()
	{
		var controller = new ProductsController(
			new StubProductService { DeleteResult = Result.Ok() }
		);

		var result = await controller.Delete(1);

		Assert.IsType<NoContentResult>(result);
	}

	[Fact]
	public async Task Categories_GetById_ReturnsNotFound_WhenMissing()
	{
		var controller = new CategoriesController(
			new StubCategoryService { GetByIdResult = new NotFoundError("missing") }
		);

		var result = await controller.GetById(999);

		Assert.IsType<NotFoundObjectResult>(result.Result);
	}

	[Fact]
	public async Task Categories_Hierarchy_ReturnsOk()
	{
		var hierarchy = new List<HierarchyCategoryResponse> { new(1, "Root", null, []) };
		var controller = new CategoriesController(
			new StubCategoryService { Hierarchy = hierarchy }
		);

		var result = await controller.Hierarchy();

		var ok = Assert.IsType<OkObjectResult>(result.Result);
		var payload = Assert.IsAssignableFrom<List<HierarchyCategoryResponse>>(ok.Value);
		Assert.Single(payload);
	}

	[Fact]
	public async Task Categories_Create_ReturnsOk_WhenSuccess()
	{
		var response = new CategoryResponse(1, "Bike", null);
		var controller = new CategoriesController(
			new StubCategoryService { CreateResult = response }
		);

		var result = await controller.Create(
			new CategoriesController.CreateCategoryRequest("Bike", null)
		);

		var ok = Assert.IsType<OkObjectResult>(result.Result);
		Assert.IsType<CategoryResponse>(ok.Value);
	}

	[Fact]
	public async Task Categories_Update_ReturnsNotFound_WhenMissing()
	{
		var controller = new CategoriesController(
			new StubCategoryService { UpdateResult = new NotFoundError("missing") }
		);

		var result = await controller.Update(
			1,
			new CategoriesController.UpdateCategoryRequest("Bike", null)
		);

		Assert.IsType<NotFoundObjectResult>(result.Result);
	}

	[Fact]
	public async Task Categories_Delete_ReturnsNoContent_WhenSuccess()
	{
		var controller = new CategoriesController(
			new StubCategoryService { DeleteResult = Result.Ok() }
		);

		var result = await controller.Delete(1);

		Assert.IsType<NoContentResult>(result);
	}

	private sealed class StubProductService : IProductService
	{
		public Result<DetailedProductResponse> GetByIdResult { get; set; } =
			new NotFoundError("not configured");
		public Result<SimpleProductResponse> CreateResult { get; set; } =
			new NotFoundError("not configured");
		public Result<DetailedProductResponse> UpdateResult { get; set; } =
			new NotFoundError("not configured");
		public Result DeleteResult { get; set; } = new NotFoundError("not configured");

		public List<SimpleProductResponse> FilterResult { get; set; } = [];
		public int CountResult { get; set; }

		public Task<Result<SimpleProductResponse>> CreateAsync(CreateProductCommand cmd)
		{
			return Task.FromResult(CreateResult);
		}

		public Task<Result<DetailedProductResponse>> GetByIDAsync(int id)
		{
			return Task.FromResult(GetByIdResult);
		}

		public Task<List<SimpleProductResponse>> FilterAsync(
			FilterProductCommand cmd,
			int page,
			int size
		)
		{
			return Task.FromResult(FilterResult);
		}

		public Task<int> CountAsync(FilterProductCommand cmd)
		{
			return Task.FromResult(CountResult);
		}

		public Task<Result<DetailedProductResponse>> UpdateAsync(UpdateProductCommand cmd)
		{
			return Task.FromResult(UpdateResult);
		}

		public Task<Result> DeleteAsync(int id)
		{
			return Task.FromResult(DeleteResult);
		}
	}

	private sealed class StubCategoryService : ICategoryService
	{
		public List<CategoryResponse> Categories { get; set; } = [];
		public List<HierarchyCategoryResponse> Hierarchy { get; set; } = [];
		public Result<CategoryResponse> GetByIdResult { get; set; } =
			new NotFoundError("not configured");
		public Result<CategoryResponse> CreateResult { get; set; } =
			new NotFoundError("not configured");
		public Result<CategoryResponse> UpdateResult { get; set; } =
			new NotFoundError("not configured");
		public Result DeleteResult { get; set; } = new NotFoundError("not configured");

		public Task<Result<CategoryResponse>> GetByIDAsync(int id)
		{
			return Task.FromResult(GetByIdResult);
		}

		public Task<List<CategoryResponse>> ListAsync()
		{
			return Task.FromResult(Categories);
		}

		public Task<List<HierarchyCategoryResponse>> ListHierarchyAsync()
		{
			return Task.FromResult(Hierarchy);
		}

		public Task<Result<CategoryResponse>> CreateAsync(CreateCategoryCommand request)
		{
			return Task.FromResult(CreateResult);
		}

		public Task<Result<CategoryResponse>> UpdateAsync(UpdateCategoryCommand request)
		{
			return Task.FromResult(UpdateResult);
		}

		public Task<Result> DeleteAsync(int id)
		{
			return Task.FromResult(DeleteResult);
		}
	}
}
