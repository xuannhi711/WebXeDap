using Microsoft.EntityFrameworkCore;

namespace WebXeDap.Application.Tests;

public static class ServiceTestHelpers
{
	public static TestApplicationDbContext CreateContext()
	{
		var options = new DbContextOptionsBuilder<TestApplicationDbContext>()
			.UseInMemoryDatabase($"test-db-{Guid.NewGuid()}")
			.Options;

		return new TestApplicationDbContext(options);
	}
}
