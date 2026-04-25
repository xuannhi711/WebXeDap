using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebXeDap.Models;
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
services.AddIdentity<ApplicationUser, IdentityRole>(
    options =>
    {
        options.User.RequireUniqueEmail = true;
    }
)
    .AddEntityFrameworkStores<ApplicationDbContext>();
services.AddLogging();
services.AddOptions();


await using var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();

var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

// Ensure role exists
if (!await roleManager.RoleExistsAsync(USER_ROLES.ADMIN))
{
    await roleManager.CreateAsync(new IdentityRole(USER_ROLES.ADMIN));
}
// Ensure user exists
var user = await userManager.FindByEmailAsync(email);

if (user != null)
{
    Console.WriteLine($"User with email {email} already exists.");
    return 1;
}


user = new ApplicationUser
{
    UserName = email.ToLower(),
    Email = email.ToLower(),
    FullName = fullName,
    EmailConfirmed = true,
    CreatedDate = DateTime.UtcNow
};

var result = await userManager.CreateAsync(user, password);
if (!result.Succeeded)
{
    Console.WriteLine(string.Join("; ", result.Errors.Select(e => e.Description)));
    return 1;
}

Console.WriteLine($"Created user: {email}");


// Ensure role assigned
if (!await userManager.IsInRoleAsync(user, USER_ROLES.ADMIN))
{
    await userManager.AddToRoleAsync(user, USER_ROLES.ADMIN);
}

Console.WriteLine("Admin user seeded successfully.");
return 0;