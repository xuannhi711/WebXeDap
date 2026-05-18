using WebXeDap.Infrastructure.Configs;
using WebXeDap.Infrastructure.Enums;

namespace WebXeDap.Infrastructure.Options;

public sealed class DbOptions
{
	public DbProvider Provider { get; }
	public string ConnectionString { get; }

	public DbOptions()
	{
		var providerStr = Environment.GetEnvironmentVariable(DevEnvVars.DB_PROVIDER);
		Provider = providerStr switch
		{
			"sqlite" => DbProvider.Sqlite,
			"mssql" => DbProvider.SqlServer,
			_ => throw new InvalidOperationException(
				"Aintno matching DB provider. Check your environment variables."
			),
		};

		var connStr = Environment.GetEnvironmentVariable(DevEnvVars.CONN_STRING);
		if (string.IsNullOrWhiteSpace(connStr))
		{
			throw new InvalidOperationException(
				$"Connection string is not set in environment variable {DevEnvVars.CONN_STRING}"
			);
		}
		ConnectionString = connStr;
	}
}
