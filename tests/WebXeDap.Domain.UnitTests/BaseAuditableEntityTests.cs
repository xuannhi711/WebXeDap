using WebXeDap.Domain.Models;

namespace WebXeDap.Domain.UnitTests;

[Trait("Category", "Unit")]
public sealed class BaseAuditableEntityTests
{
	private sealed class TestEntity : BaseAuditableEntity { }

	[Fact]
	public void Constructor_SetsCreatedAt_InValidRange()
	{
		var before = DateTime.UtcNow.AddSeconds(-5);
		var entity = new TestEntity();
		var after = DateTime.UtcNow.AddSeconds(5);

		Assert.InRange(entity.CreatedAt, before, after);
	}

	[Fact]
	public void SetUpdated_SetsUpdatedFields()
	{
		var entity = new TestEntity();
		var user = new User { Email = "test@example.com" };

		entity.SetUpdated(user: user);

		Assert.NotNull(entity.UpdatedAt);
		// Assert.Equal(42, entity.UpdatedBy?.ID);
	}

	[Fact]
	public void MarkAsDeleted_SetsDeletedFields()
	{
		var entity = new TestEntity();
		var user = new User { Email = "test@example.com" };

		entity.MarkAsDeleted(user: user);

		Assert.True(entity.IsDeleted);
		Assert.NotNull(entity.DeletedAt);
		// Assert.Equal(42, entity.UpdatedBy?.ID);
	}
}
