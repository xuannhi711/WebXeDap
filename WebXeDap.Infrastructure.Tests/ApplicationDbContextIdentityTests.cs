using Microsoft.EntityFrameworkCore;
using WebXeDap.Domain.Models;
using Xunit;

namespace WebXeDap.Infrastructure.Tests;

public sealed class ApplicationDbContextIdentityTests
{
	[Fact]
	public async Task CanPersistIdentityUser()
	{
		using var context = TestDbContextFactory.CreateContext();
		var user = new User
		{
			Id = 123,
			UserName = "admin",
			Email = "admin@example.com",
			EmailConfirmed = true,
		};

		context.Users.Add(user);
		await context.SaveChangesAsync(CancellationToken.None);

		var saved = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == 123);

		Assert.NotNull(saved);
		Assert.Equal("admin", saved!.UserName);
		Assert.Equal("admin@example.com", saved.Email);
	}
}
