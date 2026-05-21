using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebXeDap.Domain.Models;

namespace WebXeDap.WebAPI.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public sealed class UsersController : ControllerBase
{
	private readonly UserManager<User> userManager;

	public UsersController(UserManager<User> userManager)
	{
		this.userManager = userManager;
	}

	[HttpGet("me")]
	public async Task<ActionResult<UserProfileResponse>> Me()
	{
		var user = await userManager.GetUserAsync(User);
		if (user is null)
		{
			return Unauthorized();
		}
		return Ok(ToProfileResponse(user));
	}

	[HttpGet("{id:int}")]
	public async Task<ActionResult<UserProfileResponse>> GetById(int id)
	{
		var user = await userManager.FindByIdAsync(id.ToString());
		if (user is null)
		{
			return NotFound(new { message = "User not found." });
		}
		return Ok(ToProfileResponse(user));
	}

	private static UserProfileResponse ToProfileResponse(User user)
	{
		return new UserProfileResponse(user.Id, user.Email, user.FullName, user.Avatar);
	}

	public sealed record UserProfileResponse(
		int Id,
		string? Email,
		string? FullName,
		string? Avatar
	);
}
