using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebXeDap.Application.Contracts;
using WebXeDap.Application.Contracts.Persistence;

namespace WebXeDap.Application.UnitTests;

public sealed class ApplicationTestFixture
{
	public ServiceProvider Provider { get; }

	public ApplicationTestFixture()
	{
		var services = new ServiceCollection();

		services.AddApplication();

		services.AddScoped<TestCurrentUserService>();
		services.AddScoped<ICurrentUserService>(sp =>
			sp.GetRequiredService<TestCurrentUserService>()
		);

		services.AddDbContext<IApplicationDbContext, TestApplicationDbContext>(o =>
			o.UseInMemoryDatabase($"app-test-{Guid.NewGuid()}")
		);

		Provider = services.BuildServiceProvider();
	}

	public T GetService<T>()
		where T : notnull
	{
		return Provider.GetRequiredService<T>();
	}
}
