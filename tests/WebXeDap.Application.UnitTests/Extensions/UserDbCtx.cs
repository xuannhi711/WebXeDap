using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.UnitTests.Extensions;

public static class UserCtxExtensions
{
	public static async Task AddUserAsync(this IApplicationDbContext ctx, User user)
	{
		await ctx.Users.AddAsync(user);
		await ctx.SaveChangesAsync(default);
	}

	public static async Task<User> AddRandomUserAsync(this IApplicationDbContext ctx)
	{
		var user = new User { Email = $"user-{Guid.NewGuid()}@example.com" };
		await ctx.Users.AddAsync(user);
		await ctx.SaveChangesAsync(default);
		return user;
	}
}
