using System.Security.Claims;
using Util.Primitives.ResultType;
using WebXeDap.Application.Contracts;

namespace WebXeDap.WebAPI.Services;

public sealed class CurrentUserService : ICurrentUserService
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public CurrentUserService(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}

	public Result<int> UserID
	{
		get
		{
			var userIDStr = _httpContextAccessor
				.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
				?.Value;
			return int.TryParse(userIDStr, out var userID) switch
			{
				true => userID,
				false => new NotFoundError("User not found."),
			};
		}
	}
}
