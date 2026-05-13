using Microsoft.EntityFrameworkCore;
using WebXeDap.Infrastructure;

namespace WebXeDap.Infrastructure.Tests;

public static class TestDbContextFactory
{
	public static ApplicationDbContext CreateContext()
	{
		var options = new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseInMemoryDatabase($"infra-test-{Guid.NewGuid()}")
			.Options;

		return new ApplicationDbContext(options);
	}
}
