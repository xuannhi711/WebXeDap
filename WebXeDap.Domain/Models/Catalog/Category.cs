namespace WebXeDap.Domain.Models.Catalog;

public class Category
{
	public int ID { get; set; }
	public required string Name { get; set; }

	// self-ref relationship
	public int? ParentCategoryID { get; set; }
	public Category? ParentCategory { get; set; }
	public ICollection<Category> Children { get; set; } = [];

	// many-to-many with Product
	public ICollection<Product> Products { get; set; } = [];
}
