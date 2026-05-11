using Microsoft.Extensions.DependencyInjection;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Tests.Infrastructure;

namespace WebXeDap.Application.Tests.Fixtures;

public sealed class ApplicationTestFixture
{
	public ServiceProvider Provider { get; }

	public ApplicationTestFixture()
	{
		var services = new ServiceCollection();

		services.AddApplication();

		services.AddScoped<IApplicationDbContext>(_ =>
			TestApplicationDbContextFactory.CreateContext()
		);

		Provider = services.BuildServiceProvider();
	}

	public T GetService<T>()
		where T : notnull
	{
		return Provider.GetRequiredService<T>();
	}
}
