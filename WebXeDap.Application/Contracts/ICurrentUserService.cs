using Util.Primitives.ResultType;

namespace WebXeDap.Application.Contracts;

public interface ICurrentUserService
{
	Result<int> UserID { get; }
}
