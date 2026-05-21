using Microsoft.EntityFrameworkCore;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Domain.Models;

namespace WebXeDap.Infrastructure.UnitTests;

[Trait("Category", "Unit")]
public sealed class ApplicationDbContextIdentityTests
{
	private readonly IApplicationDbContext ctx;

	public ApplicationDbContextIdentityTests()
	{
		var fixture = new InfraTestFixture();
		ctx = fixture.GetService<IApplicationDbContext>();
	}

	[Fact]
	public async Task CanPersistIdentityUser()
	{
		var user = new User
		{
			Id = 123,
			UserName = "admin",
			Email = "admin@example.com",
			EmailConfirmed = true,
		};

		ctx.Users.Add(user);
		await ctx.SaveChangesAsync(CancellationToken.None);

		var saved = await ctx.Users.FindAsync(123);

		Assert.NotNull(saved);
		Assert.Equal("admin", saved!.UserName);
		Assert.Equal("admin@example.com", saved.Email);
	}
}
