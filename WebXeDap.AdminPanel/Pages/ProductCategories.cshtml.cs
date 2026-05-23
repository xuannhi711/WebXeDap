using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebXeDap.Application.Contracts.Persistence;

namespace WebXeDap.AdminPanel.Pages;

public sealed class ProductCategoriesModel : PageModel
{
	private readonly IApplicationDbContext _ctx;

	public ProductCategoriesModel(IApplicationDbContext ctx)
	{
		_ctx = ctx;
	}

	[BindProperty(SupportsGet = true)]
	public int ProductId { get; set; }

	public List<(int Id, string Name)> Products { get; set; } = new();
	public List<(int Id, string Name)> Categories { get; set; } = new();

	[BindProperty]
	public List<int> SelectedCategoryIds { get; set; } = new();

	public async Task OnGetAsync()
	{
		var prodList = await _ctx
			.Products.AsNoTracking()
			.Select(p => new { p.ID, p.Name })
			.ToListAsync();

		Products = prodList.Select(x => (x.ID, x.Name)).ToList();

		var catList = await _ctx
			.Categories.AsNoTracking()
			.Select(c => new { c.ID, c.Name })
			.ToListAsync();

		Categories = catList.Select(x => (x.ID, x.Name)).ToList();

		if (ProductId > 0)
		{
			var product = await _ctx
				.Products.Include(p => p.Categories)
				.FirstOrDefaultAsync(p => p.ID == ProductId);
			if (product != null)
			{
				SelectedCategoryIds = product.Categories.Select(c => c.ID).ToList();
			}
		}
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (ProductId <= 0)
			return Page();

		var product = await _ctx
			.Products.Include(p => p.Categories)
			.FirstOrDefaultAsync(p => p.ID == ProductId);
		if (product == null)
			return NotFound();

		// load selected categories
		var cats = await _ctx
			.Categories.Where(c => SelectedCategoryIds.Contains(c.ID))
			.ToListAsync();

		// replace associations
		product.Categories.Clear();
		foreach (var c in cats)
			product.Categories.Add(c);

		await _ctx.SaveChangesAsync(default);

		return RedirectToPage(new { ProductId });
	}
}
