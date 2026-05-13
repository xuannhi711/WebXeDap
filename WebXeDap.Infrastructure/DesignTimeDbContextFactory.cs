using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WebXeDap.Infrastructure;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
	public ApplicationDbContext CreateDbContext(string[] args)
	{
		var provider = Environment.GetEnvironmentVariable("DB_PROVIDER");
		var useSqlite = string.Equals(provider, "sqlite", StringComparison.OrdinalIgnoreCase);
		var migrationsAssembly = typeof(ApplicationDbContext).Assembly.FullName;

		var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

		if (useSqlite)
		{
			var sqliteConnection =
				Environment.GetEnvironmentVariable("SQLITE_CONNECTION_STRING")
				?? "Data Source=webxedap.db";

			optionsBuilder.UseSqlite(
				sqliteConnection,
				sqlite => sqlite.MigrationsAssembly(migrationsAssembly)
			);
		}
		else
		{
			var sqlServerConnection = Environment.GetEnvironmentVariable(
				"ConnectionStrings__DefaultConnection"
			);

			if (string.IsNullOrWhiteSpace(sqlServerConnection))
			{
				throw new InvalidOperationException(
					"ConnectionStrings__DefaultConnection is not set for SQL Server migrations."
				);
			}

			optionsBuilder.UseSqlServer(
				sqlServerConnection,
				sql => sql.MigrationsAssembly(migrationsAssembly)
			);
		}

		return new ApplicationDbContext(optionsBuilder.Options);
	}
}
