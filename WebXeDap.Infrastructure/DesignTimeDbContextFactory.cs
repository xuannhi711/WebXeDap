using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace WebXeDap.Infrastructure;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
	public ApplicationDbContext CreateDbContext(string[] args)
	{
		var services = new ServiceCollection();

		services.AddInfrastructure();

		var provider = services.BuildServiceProvider();

		return provider.GetRequiredService<ApplicationDbContext>();
	}
}
