using WebXeDap.Application.Common.Interfaces;

namespace WebXeDap.Application.Common.Extensions;

public static class CurrentUserServiceExtensions
{
	public static int GetRequiredUserId(this ICurrentUserService currentUser)
	{
		if (currentUser.UserId is null)
		{
			throw new UnauthorizedAccessException("User is not authenticated.");
		}

		return currentUser.UserId.Value;
	}
}
