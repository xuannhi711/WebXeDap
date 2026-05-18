namespace WebXeDap.Infrastructure.Options;

using WebXeDap.Infrastructure.Configs;
using E = Environment;

public sealed class EmailOptions
{
	public required string Host { get; init; }

	public int Port { get; init; }

	public required string Username { get; init; }

	public required string Password { get; init; }

	public required string FromEmail { get; init; }

	public required string FromName { get; init; }

	public bool UseSSL { get; init; } = false;

	public static EmailOptions LoadFromEnvironment()
	{
		var host =
			E.GetEnvironmentVariable(DevEnvVars.SMTP_HOST)
			?? throw new InvalidOperationException($"No env {DevEnvVars.SMTP_HOST} is set.");
		var portStr =
			E.GetEnvironmentVariable(DevEnvVars.SMTP_PORT)
			?? throw new InvalidOperationException($"No env {DevEnvVars.SMTP_PORT} is set.");
		if (!int.TryParse(portStr, out var port))
		{
			throw new InvalidOperationException(
				$"Env {DevEnvVars.SMTP_PORT} is not a valid integer."
			);
		}
		var username =
			E.GetEnvironmentVariable(DevEnvVars.SMTP_USER)
			?? throw new InvalidOperationException($"No env {DevEnvVars.SMTP_USER} is set.");
		var password =
			E.GetEnvironmentVariable(DevEnvVars.SMTP_PASS)
			?? throw new InvalidOperationException($"No env {DevEnvVars.SMTP_PASS} is set.");
		var fromEmail =
			E.GetEnvironmentVariable(DevEnvVars.SMTP_FROM_EMAIL)
			?? throw new InvalidOperationException($"No env {DevEnvVars.SMTP_FROM_EMAIL} is set.");
		var fromName =
			E.GetEnvironmentVariable(DevEnvVars.SMTP_FROM_NAME)
			?? throw new InvalidOperationException($"No env {DevEnvVars.SMTP_FROM_NAME} is set.");
		return new EmailOptions
		{
			Host = host,
			Port = port,
			Username = username,
			Password = password,
			FromEmail = fromEmail,
			FromName = fromName,
		};
	}
}
