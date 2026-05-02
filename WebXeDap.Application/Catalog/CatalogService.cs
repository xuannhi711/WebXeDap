using Microsoft.EntityFrameworkCore;
using WebXeDap.Application.Common.Exceptions;
using WebXeDap.Application.Common.Interfaces;
using WebXeDap.Application.Common.Models;
using WebXeDap.Application.Catalog.Mappings;
using WebXeDap.Application.Catalog.Models;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Catalog;

public sealed class CatalogService
{
	private readonly IApplicationDbContext _context;
	private readonly ICurrentUserService _currentUser;

	public CatalogService(IApplicationDbContext context, ICurrentUserService currentUser)
	{
		_context = context;
		_currentUser = currentUser;
	}

	public async Task<int> CreateCategoryAsync(
		string name,
		int? parentCategoryId,
		CancellationToken cancellationToken
	)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentException("Category name is required.", nameof(name));
		}

		Category? parent = null;
		if (parentCategoryId.HasValue)
		{
			parent = await _context.Categories
				.FirstOrDefaultAsync(c => c.ID == parentCategoryId.Value, cancellationToken);

			if (parent is null)
			{
				throw new NotFoundException(nameof(Category), parentCategoryId.Value);
			}
		}

		var category = new Category
		{
			Name = name.Trim(),
			ParentCategoryID = parentCategoryId,
			ParentCategory = parent
		};

		_context.Categories.Add(category);
		await _context.SaveChangesAsync(cancellationToken);

		return category.ID;
	}

	public async Task UpdateCategoryAsync(
		int id,
		string name,
		int? parentCategoryId,
		CancellationToken cancellationToken
	)
	{
		if (id == parentCategoryId)
		{
			throw new InvalidOperationException("A category cannot be its own parent.");
		}

		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentException("Category name is required.", nameof(name));
		}

		var category = await _context.Categories
			.FirstOrDefaultAsync(c => c.ID == id, cancellationToken);

		if (category is null)
		{
			throw new NotFoundException(nameof(Category), id);
		}

		Category? parent = null;
		if (parentCategoryId.HasValue)
		{
			parent = await _context.Categories
				.FirstOrDefaultAsync(c => c.ID == parentCategoryId.Value, cancellationToken);

			if (parent is null)
			{
				throw new NotFoundException(nameof(Category), parentCategoryId.Value);
			}
		}

		category.Name = name.Trim();
		category.ParentCategoryID = parentCategoryId;
		category.ParentCategory = parent;

		await _context.SaveChangesAsync(cancellationToken);
	}

	public async Task DeleteCategoryAsync(int id, CancellationToken cancellationToken)
	{
		var category = await _context.Categories
			.Include(c => c.Children)
			.Include(c => c.Products)
			.FirstOrDefaultAsync(c => c.ID == id, cancellationToken);

		if (category is null)
		{
			throw new NotFoundException(nameof(Category), id);
		}

		if (category.Children.Count > 0 || category.Products.Count > 0)
		{
			throw new InvalidOperationException("Category is in use and cannot be deleted.");
		}

		_context.Categories.Remove(category);
		await _context.SaveChangesAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken)
	{
		var categories = await _context.Categories
			.AsNoTracking()
			.OrderBy(c => c.Name)
			.ToListAsync(cancellationToken);

		return categories.Select(c => c.ToDto()).ToList();
	}

	public async Task<CategoryDto> GetCategoryByIdAsync(int id, CancellationToken cancellationToken)
	{
		var category = await _context.Categories
			.AsNoTracking()
			.FirstOrDefaultAsync(c => c.ID == id, cancellationToken);

		if (category is null)
		{
			throw new NotFoundException(nameof(Category), id);
		}

		return category.ToDto();
	}

	public async Task<int> CreateProductAsync(
		string name,
		string? description,
		decimal price,
		string? currencySymbol,
		int quantity,
		IReadOnlyList<int>? categoryIds,
		IReadOnlyList<ProductImageInput>? images,
		CancellationToken cancellationToken
	)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentException("Product name is required.", nameof(name));
		}

		if (price < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(price), "Price must be non-negative.");
		}

		if (quantity < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be non-negative.");
		}

		var requestedCategoryIds = categoryIds ?? Array.Empty<int>();
		var categories = new List<Category>();
		if (requestedCategoryIds.Count > 0)
		{
			categories = await _context.Categories
				.Where(c => requestedCategoryIds.Contains(c.ID))
				.ToListAsync(cancellationToken);

			if (categories.Count != requestedCategoryIds.Count)
			{
				var missing = requestedCategoryIds.Except(categories.Select(c => c.ID));
				throw new NotFoundException(nameof(Category), string.Join(",", missing));
			}
		}

		var product = new Product
		{
			Name = name.Trim(),
			Description = description?.Trim(),
			Price = price,
			CurrencySymbol = string.IsNullOrWhiteSpace(currencySymbol)
				? "VNĐ"
				: currencySymbol.Trim(),
			Quantity = quantity
		};

		foreach (var category in categories)
		{
			product.Categories.Add(category);
		}

		var imageInputs = images ?? Array.Empty<ProductImageInput>();
		foreach (var image in imageInputs.Where(i => !string.IsNullOrWhiteSpace(i.Key)))
		{
			product.Images.Add(new ProductImage
			{
				key = image.Key.Trim(),
				Order = image.Order,
				Product = product
			});
		}

		_context.Products.Add(product);
		await _context.SaveChangesAsync(cancellationToken);

		return product.ID;
	}

	public async Task UpdateProductAsync(
		int id,
		string name,
		string? description,
		decimal price,
		string? currencySymbol,
		int quantity,
		IReadOnlyList<int>? categoryIds,
		IReadOnlyList<ProductImageInput>? images,
		CancellationToken cancellationToken
	)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentException("Product name is required.", nameof(name));
		}

		if (price < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(price), "Price must be non-negative.");
		}

		if (quantity < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be non-negative.");
		}

		var product = await _context.Products
			.Include(p => p.Categories)
			.Include(p => p.Images)
			.FirstOrDefaultAsync(p => p.ID == id, cancellationToken);

		if (product is null || product.IsDeleted)
		{
			throw new NotFoundException(nameof(Product), id);
		}

		var requestedCategoryIds = categoryIds ?? Array.Empty<int>();
		var categories = new List<Category>();
		if (requestedCategoryIds.Count > 0)
		{
			categories = await _context.Categories
				.Where(c => requestedCategoryIds.Contains(c.ID))
				.ToListAsync(cancellationToken);

			if (categories.Count != requestedCategoryIds.Count)
			{
				var missing = requestedCategoryIds.Except(categories.Select(c => c.ID));
				throw new NotFoundException(nameof(Category), string.Join(",", missing));
			}
		}

		product.Name = name.Trim();
		product.Description = description?.Trim();
		product.Price = price;
		product.CurrencySymbol = string.IsNullOrWhiteSpace(currencySymbol)
			? "VNĐ"
			: currencySymbol.Trim();
		product.Quantity = quantity;

		product.Categories.Clear();
		foreach (var category in categories)
		{
			product.Categories.Add(category);
		}

		_context.ProductImages.RemoveRange(product.Images);
		product.Images.Clear();

		var imageInputs = images ?? Array.Empty<ProductImageInput>();
		foreach (var image in imageInputs.Where(i => !string.IsNullOrWhiteSpace(i.Key)))
		{
			product.Images.Add(new ProductImage
			{
				key = image.Key.Trim(),
				Order = image.Order,
				Product = product
			});
		}

		product.SetUpdated(_currentUser.UserId);
		await _context.SaveChangesAsync(cancellationToken);
	}

	public async Task DeleteProductAsync(int id, CancellationToken cancellationToken)
	{
		var product = await _context.Products
			.FirstOrDefaultAsync(p => p.ID == id, cancellationToken);

		if (product is null || product.IsDeleted)
		{
			throw new NotFoundException(nameof(Product), id);
		}

		product.MarkAsDeleted(_currentUser.UserId);
		await _context.SaveChangesAsync(cancellationToken);
	}

	public async Task<ProductDto> GetProductByIdAsync(int id, CancellationToken cancellationToken)
	{
		var product = await _context.Products
			.AsNoTracking()
			.Include(p => p.Categories)
			.Include(p => p.Images)
			.FirstOrDefaultAsync(p => p.ID == id && !p.IsDeleted, cancellationToken);

		if (product is null)
		{
			throw new NotFoundException(nameof(Product), id);
		}

		return product.ToDto();
	}

	public async Task<PagedResult<ProductDto>> SearchProductsAsync(
		string? searchTerm,
		int? categoryId,
		decimal? minPrice,
		decimal? maxPrice,
		string? sortBy,
		bool sortDescending,
		int pageNumber,
		int pageSize,
		CancellationToken cancellationToken
	)
	{
		var query = _context.Products
			.AsNoTracking()
			.Where(p => !p.IsDeleted);

		if (!string.IsNullOrWhiteSpace(searchTerm))
		{
			var term = searchTerm.Trim();
			query = query.Where(p =>
				p.Name.Contains(term) || (p.Description != null && p.Description.Contains(term))
			);
		}

		if (categoryId.HasValue)
		{
			query = query.Where(p => p.Categories.Any(c => c.ID == categoryId.Value));
		}

		if (minPrice.HasValue)
		{
			query = query.Where(p => p.Price >= minPrice.Value);
		}

		if (maxPrice.HasValue)
		{
			query = query.Where(p => p.Price <= maxPrice.Value);
		}

		var normalizedSortBy = sortBy?.Trim().ToLowerInvariant();
		query = normalizedSortBy switch
		{
			"price" => sortDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
			"name" => sortDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
			"created" => sortDescending
				? query.OrderByDescending(p => p.CreatedAt)
				: query.OrderBy(p => p.CreatedAt),
			_ => query.OrderBy(p => p.ID)
		};

		var normalizedPageNumber = pageNumber < 1 ? 1 : pageNumber;
		var normalizedPageSize = pageSize < 1 ? 20 : pageSize;

		var totalCount = await query.CountAsync(cancellationToken);
		var items = await query
			.Include(p => p.Categories)
			.Include(p => p.Images)
			.Skip((normalizedPageNumber - 1) * normalizedPageSize)
			.Take(normalizedPageSize)
			.ToListAsync(cancellationToken);

		var results = items.Select(p => p.ToDto()).ToList();
		return new PagedResult<ProductDto>(results, totalCount, normalizedPageNumber, normalizedPageSize);
	}
}
