using WebXeDap.Application.Common.Interfaces;

namespace WebXeDap.Application.Tests;

public sealed class TestCurrentUserService : ICurrentUserService
{
	public int? UserId { get; set; }
}
