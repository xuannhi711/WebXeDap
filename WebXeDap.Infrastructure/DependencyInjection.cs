using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Domain.Models;
using WebXeDap.Infrastructure.Enums;
using WebXeDap.Infrastructure.Options;

namespace WebXeDap.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services)
	{
		var migrationsAssembly = typeof(ApplicationDbContext).Assembly.FullName;
		var dbOptions = new DbOptions();

		_ = dbOptions.Provider switch
		{
			DbProvider.Sqlite => services.AddDbContext<ApplicationDbContext>(o =>
				o.UseSqlite(
					dbOptions.ConnectionString,
					sqlite => sqlite.MigrationsAssembly(migrationsAssembly)
				)
			),
			DbProvider.SqlServer => services.AddDbContext<ApplicationDbContext>(o =>
				o.UseSqlServer(
					dbOptions.ConnectionString,
					sql => sql.MigrationsAssembly(migrationsAssembly)
				)
			),
			_ => throw new InvalidOperationException("Unsupported database provider"),
		};

		services
			.AddIdentityCore<User>(options =>
			{
				options.User.RequireUniqueEmail = true;
			})
			.AddRoles<IdentityRole<int>>()
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddDefaultTokenProviders();

		services.AddScoped<IApplicationDbContext>(sp =>
			sp.GetRequiredService<ApplicationDbContext>()
		);

		return services;
	}
}
