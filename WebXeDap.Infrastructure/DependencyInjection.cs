using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Domain.Models;

namespace WebXeDap.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services)
	{
		var conf = new InfrastructureConfiguration();
		var migrationsAssembly = typeof(ApplicationDbContext).Assembly.FullName;

		if (conf.IsSqlite)
		{
			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlite(
					conf.ConnectionString,
					sqlite => sqlite.MigrationsAssembly(migrationsAssembly)
				)
			);
		}
		else
		{
			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(
					conf.ConnectionString,
					sql => sql.MigrationsAssembly(migrationsAssembly)
				)
			);
		}

		services
			.AddIdentity<User, IdentityRole<int>>(options =>
			{
				options.User.RequireUniqueEmail = true;
			})
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddDefaultTokenProviders();

		services.AddScoped<IApplicationDbContext>(sp =>
			sp.GetRequiredService<ApplicationDbContext>()
		);

		return services;
	}
}
