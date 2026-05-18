using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebXeDap.Domain.Constants;
using WebXeDap.Domain.Models;
using WebXeDap.Infrastructure;
using WebXeDap.Seeder;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure();

var host = builder.Build();

using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;

var dbContext = services.GetRequiredService<ApplicationDbContext>();
var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
var userManager = services.GetRequiredService<UserManager<User>>();

Console.WriteLine("====================== ADMIN SEEDER ======================");

var adminResult = await AdminSeeder.CreateAdmin(services);
if (!adminResult.TryPickValue(out var admin, out var err))
{
	System.Console.WriteLine($"Failed to create admin user: {err}");
	return 1;
}

Console.WriteLine($"Admin {admin.Email} seeded successfully.");
return 0;
