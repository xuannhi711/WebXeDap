using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebXeDap.Application.Contracts.Persistence;

namespace WebXeDap.Infrastructure.UnitTests;

public sealed class InfraTestFixture
{
	public ServiceProvider Provider { get; }

	public InfraTestFixture()
	{
		var services = new ServiceCollection();

		services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(o =>
			o.UseInMemoryDatabase($"infra-test-{Guid.NewGuid()}")
		);

		Provider = services.BuildServiceProvider();
	}

	public T GetService<T>()
		where T : notnull
	{
		return Provider.GetRequiredService<T>();
	}
}
