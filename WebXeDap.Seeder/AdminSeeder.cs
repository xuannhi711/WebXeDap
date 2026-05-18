using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Util.Primitives.ResultType;
using WebXeDap.Domain.Constants;
using WebXeDap.Domain.Models;

namespace WebXeDap.Seeder;

internal static class AdminSeeder
{
	public static async Task<Result<User>> CreateAdmin(IServiceProvider provider)
	{
		var roleManager = provider.GetRequiredService<RoleManager<IdentityRole<int>>>();
		var userManager = provider.GetRequiredService<UserManager<User>>();

		var roles = new[] { ROLES.ADMIN };
		foreach (var role in roles)
		{
			if (!await roleManager.RoleExistsAsync(role))
			{
				await roleManager.CreateAsync(new IdentityRole<int>(role));
			}
		}

		var email = Prompter.PromptRequired("Admin email");
		var password = Prompter.PromptRequired("Admin password");
		var fullName = Prompter.PromptRequired("Admin full name");

		var user = new User
		{
			UserName = email.Trim(),
			Email = email.Trim(),
			EmailConfirmed = true,
		};

		var result = await userManager.CreateAsync(user, password);
		if (!result.Succeeded)
		{
			return new ValidationError(
				result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description })
			);
		}

		result = await userManager.AddToRoleAsync(user, ROLES.ADMIN);
		if (!result.Succeeded)
		{
			await userManager.DeleteAsync(user);
			return new ValidationError(
				result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description })
			);
		}

		return user;
	}
}
