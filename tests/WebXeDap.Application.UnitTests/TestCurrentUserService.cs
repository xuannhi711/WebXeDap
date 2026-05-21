using Util.Primitives.ResultType;
using WebXeDap.Application.Contracts;

namespace WebXeDap.Application.UnitTests;

public sealed class TestCurrentUserService : ICurrentUserService
{
	public Result<int> UserID { get; set; } = new UnauthorizedError("User is not authenticated.");
}
