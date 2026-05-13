using System.Security.Claims;
using WebXeDap.Application.Contracts;

namespace WebXeDap.WebAPI.Services;

public sealed class CurrentUserService : ICurrentUserService
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public CurrentUserService(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}

	public int? UserId
	{
		get
		{
			var user = _httpContextAccessor.HttpContext?.User;
			if (user is null)
			{
				return null;
			}

			var sub =
				user.FindFirst("sub")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			return int.TryParse(sub, out var userId) ? userId : null;
		}
	}
}
