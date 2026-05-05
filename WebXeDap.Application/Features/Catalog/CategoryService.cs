using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Features.Catalog;

public class CategoryService : ICategoryService
{
	private readonly ICategoryRepository _categoryRepo;

	public CategoryService(ICategoryRepository categoryRepo)
	{
		_categoryRepo = categoryRepo;
	}

	public Task<int> CreateAsync(CreateCategoryRequest request, CancellationToken ct = default)
	{
		throw new NotImplementedException();
	}

	public async Task<int> CreateCategoryAsync(
		CreateCategoryRequest req,
		CancellationToken cancellationToken
	)
	{
		var category = new Category
		{
			Name = req.Name.Trim(),
			ParentCategoryID = req.ParentCategoryID,
		};

		await _categoryRepo.AddAsync(category, cancellationToken);
		return category.ID;
	}

	public Task DeleteAsync(DeleteCategoryRequest request, CancellationToken ct = default)
	{
		throw new NotImplementedException();
	}

	public Task<List<CategoryResponse>> GetAllAsync(CancellationToken ct = default)
	{
		throw new NotImplementedException();
	}

	public Task<CategoryResponse?> GetByIDAsync(int id, CancellationToken ct = default)
	{
		throw new NotImplementedException();
	}

	public Task<List<CategoryResponse>> GetHierarchyAsync(CancellationToken ct = default)
	{
		throw new NotImplementedException();
	}

	public Task UpdateAsync(UpdateCategoryRequest request, CancellationToken ct = default)
	{
		throw new NotImplementedException();
	}
}
