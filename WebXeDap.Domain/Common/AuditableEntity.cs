namespace WebXeDap.Domain.Common;

public abstract class AuditableEntity
{
	public DateTime CreatedAt { get; protected set; }
	public DateTime? UpdatedAt { get; protected set; }

	public int CreatedBy { get; protected set; }
	public int? UpdatedBy { get; protected set; }

	public bool IsDeleted { get; protected set; }
	public DateTime? DeletedAt { get; protected set; }

	protected AuditableEntity()
	{
		CreatedAt = DateTime.UtcNow;
	}

	public void SetUpdated(int? userId = null)
	{
		UpdatedAt = DateTime.UtcNow;
		UpdatedBy = userId;
	}

	public void MarkAsDeleted(int? userId = null)
	{
		IsDeleted = true;
		DeletedAt = DateTime.UtcNow;
		UpdatedBy = userId;
	}
}
