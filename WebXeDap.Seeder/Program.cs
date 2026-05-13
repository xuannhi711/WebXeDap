using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebXeDap.Domain.Constants;
using WebXeDap.Domain.Models;
using WebXeDap.Infrastructure;
using WebXeDap.Seeder;

Console.WriteLine("====================== ADMIN SEEDER ======================");
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
if (string.IsNullOrEmpty(connectionString))
{
	Console.WriteLine("Error: CONNECTION_STRING environment variable is not set.");
	return 1;
}

var email = Prompter.PromptRequired("Admin email");
var password = Prompter.PromptRequired("Admin password");
var fullName = Prompter.Prompt("Admin full name", "System Admin");

var services = new ServiceCollection();
services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
services
	.AddIdentityCore<User>(options =>
	{
		options.User.RequireUniqueEmail = true;
	})
	.AddRoles<IdentityRole<int>>()
	.AddEntityFrameworkStores<ApplicationDbContext>();
services.AddLogging();
services.AddOptions();

await using var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();

var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

// Ensure roles exist
var roles = new[] { ROLES.ADMIN, ROLES.STAFF, ROLES.CUSTOMER };
foreach (var role in roles)
{
	if (!await roleManager.RoleExistsAsync(role))
	{
		await roleManager.CreateAsync(new IdentityRole<int>(role));
	}
}

// Ensure user exists
var user = await userManager.FindByEmailAsync(email);

if (user != null)
{
	Console.WriteLine($"User with email {email} already exists.");
	return 1;
}

user = new User
{
	UserName = email.ToLower(),
	Email = email.ToLower(),
	FullName = fullName,
	EmailConfirmed = true,
};

var result = await userManager.CreateAsync(user, password);
if (!result.Succeeded)
{
	Console.WriteLine(string.Join("; ", result.Errors.Select(e => e.Description)));
	return 1;
}

Console.WriteLine($"Created user: {email}");

// Ensure role assigned
if (!await userManager.IsInRoleAsync(user, ROLES.ADMIN))
{
	await userManager.AddToRoleAsync(user, ROLES.ADMIN);
}

Console.WriteLine("Admin user seeded successfully.");
return 0;
