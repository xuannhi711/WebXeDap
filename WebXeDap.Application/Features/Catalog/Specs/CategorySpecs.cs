using Ardalis.Specification;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Features.Catalog.Specs;

public class CategoryByNameSpec : Specification<Category>
{
	public string Name { get; }

	public CategoryByNameSpec(string name)
	{
		Name = name;
		Query.Where(c => c.Name.Contains(name));
	}
}

public class CategoryByIDSpec : Specification<Category>
{
	public int ID { get; }

	public CategoryByIDSpec(int id)
	{
		ID = id;
		Query.Where(c => c.ID == id);
	}
}
