namespace WebXeDap.Domain.Models;

public abstract class BaseAuditableEntity
{
	public DateTime CreatedAt { get; protected set; }
	public DateTime? UpdatedAt { get; protected set; }

	// public int? CreatedById { get; protected set; }
	// public int? UpdatedById { get; protected set; }
	// public User? CreatedBy { get; protected set; }
	// public User? UpdatedBy { get; protected set; }

	public bool IsDeleted { get; protected set; }
	public DateTime? DeletedAt { get; protected set; }

	// protected để lớp con kế thừa và gọi
	protected BaseAuditableEntity()
	{
		CreatedAt = DateTime.UtcNow;
	}

	public void SetUpdated(User? user = null)
	{
		UpdatedAt = DateTime.UtcNow;
		// UpdatedBy = user;
	}

	public void MarkAsDeleted(User? user = null)
	{
		IsDeleted = true;
		DeletedAt = DateTime.UtcNow;
		// UpdatedBy = user;
	}
}
