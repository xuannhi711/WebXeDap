using System.Linq;
using Util.Primitives.ResultType;
using WebXeDap.Infrastructure.Options;

namespace WebXeDap.Infrastructure;

public class InfrastructureConfiguration
{
	private static readonly HashSet<string> VALID_PROVIDERS = new(StringComparer.OrdinalIgnoreCase)
	{
		"sqlite",
		"sqlserver",
		"mssql",
	};
	public bool IsSqlite { get; }
	public string ConnectionString { get; }

	public InfrastructureConfiguration()
	{
		var provider = Environment.GetEnvironmentVariable(EnvVarOptions.DbProviderVar);

		if (!string.IsNullOrWhiteSpace(provider) && !VALID_PROVIDERS.Contains(provider))
		{
			throw new InvalidOperationException(
				$"{provider} aint in ({string.Join("|", VALID_PROVIDERS)})"
			);
		}

		IsSqlite = provider == "sqlite";

		ConnectionString = Environment.GetEnvironmentVariable(EnvVarOptions.ConnectionStringVar)!;
	}
}
