using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Domain.Models;
using WebXeDap.Infrastructure.Enums;
using WebXeDap.Infrastructure.Options;
using WebXeDap.Infrastructure.Services;

namespace WebXeDap.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services)
	{
		var migrationsAssembly = typeof(ApplicationDbContext).Assembly.FullName;
		var dbOptions = DbOptions.LoadFromEnvironment();

		services.AddSingleton(dbOptions);

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
			.AddIdentity<User, IdentityRole<int>>(options =>
			{
				options.User.RequireUniqueEmail = true;
			})
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddDefaultTokenProviders()
			.AddApiEndpoints();

		services.AddScoped<IApplicationDbContext>(sp =>
			sp.GetRequiredService<ApplicationDbContext>()
		);

		services.AddSingleton(EmailOptions.LoadFromEnvironment());
		services.AddTransient<IEmailSender<User>, EmailService>();

		return services;
	}
}
