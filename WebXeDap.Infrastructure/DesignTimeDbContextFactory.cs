using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WebXeDap.Infrastructure;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
	public ApplicationDbContext CreateDbContext(string[] args)
	{
		var conf = new InfrastructureConfiguration();
		var migrationsAssembly = typeof(ApplicationDbContext).Assembly.FullName;

		var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

		if (conf.IsSqlite)
		{
			optionsBuilder.UseSqlite(
				conf.ConnectionString,
				sqlite => sqlite.MigrationsAssembly(migrationsAssembly)
			);
		}
		else
		{
			optionsBuilder.UseSqlServer(
				conf.ConnectionString,
				sql => sql.MigrationsAssembly(migrationsAssembly)
			);
		}

		return new ApplicationDbContext(optionsBuilder.Options);
	}
}
