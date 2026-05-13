using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Domain.Models;

namespace WebXeDap.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(
		this IServiceCollection services,
		string connectionString,
		bool useSqlite = false
	)
	{
		var migrationsAssembly = typeof(ApplicationDbContext).Assembly.FullName;

		if (useSqlite)
		{
			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlite(
					connectionString,
					sqlite => sqlite.MigrationsAssembly(migrationsAssembly)
				)
			);
		}
		else
		{
			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(
					connectionString,
					sql => sql.MigrationsAssembly(migrationsAssembly)
				)
			);
		}

		services
			.AddIdentity<User, IdentityRole<int>>()
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddDefaultTokenProviders();

		services.AddScoped<IApplicationDbContext>(sp =>
			sp.GetRequiredService<ApplicationDbContext>()
		);

		return services;
	}
}
