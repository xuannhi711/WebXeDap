using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebXeDap.Domain.Models;
using WebXeDap.Infrastructure;
using WebXeDap.Seeder;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure();

builder.Logging.ClearProviders();

builder.Logging.AddConsole();

builder.Logging.SetMinimumLevel(LogLevel.Warning);

var host = builder.Build();

using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;

var dbContext = services.GetRequiredService<ApplicationDbContext>();
var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
var userManager = services.GetRequiredService<UserManager<User>>();

if (args.Length == 0)
{
	Console.WriteLine("Usage:");
	Console.WriteLine("  dotnet run createadmin");
	Console.WriteLine("  dotnet run db");

	return 1;
}

switch (args[0].ToLower())
{
	case "createadmin":
	{
		Console.WriteLine("====================== ADMIN SEEDER ======================");

		var adminResult = await AdminSeeder.CreateAdmin(services);

		if (!adminResult.TryPickValue(out var admin, out var err))
		{
			Console.WriteLine($"Failed to create admin user: {err}");
			return 1;
		}

		Console.WriteLine($"Admin {admin.Email} seeded successfully.");
		break;
	}

	case "db":
	{
		Console.WriteLine("====================== DB SEEDER ======================");

		await DbSeeder.SeedAsync(services);

		Console.WriteLine("Database seeded successfully.");
		break;
	}

	default:
	{
		Console.WriteLine($"Unknown command: {args[0]}");
		return 1;
	}
}

return 0;
