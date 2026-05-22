using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebXeDap.Domain.Models;
using WebXeDap.WebAPI.Exchange;

namespace WebXeDap.WebAPI.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public sealed class UsersController : ControllerBase
{
	private readonly UserManager<User> userManager;
	private readonly SignInManager<User> signInManager;

	public UsersController(UserManager<User> userManager, SignInManager<User> signInManager)
	{
		this.userManager = userManager;
		this.signInManager = signInManager;
	}

	[HttpGet("me")]
	public async Task<ActionResult<UserProfileResp>> Me()
	{
		var user = await userManager.GetUserAsync(User);
		if (user is null)
		{
			return Unauthorized();
		}
		return Ok(UserExchangeMapper.ToUserProfileResp(user));
	}

	[HttpGet("{id:int}")]
	public async Task<ActionResult<UserProfileResp>> GetByID(int id)
	{
		var user = await userManager.FindByIdAsync(id.ToString());
		if (user is null)
		{
			return NotFound(new { message = "User not found." });
		}
		return Ok(UserExchangeMapper.ToUserProfileResp(user));
	}

	[HttpPut("me")]
	public async Task<ActionResult<UserProfileResp>> UpdateMe([FromBody] UpdateUserProfileReq req)
	{
		var user = await userManager.GetUserAsync(User);
		if (user is null)
		{
			return Unauthorized();
		}
		UserExchangeMapper.PatchUser(req, user);
		var result = await userManager.UpdateAsync(user);
		if (!result.Succeeded)
		{
			return BadRequest(result.Errors);
		}
		return Ok(UserExchangeMapper.ToUserProfileResp(user));
	}

	[HttpPost("logout")]
	public async Task<ActionResult> Logout()
	{
		await signInManager.SignOutAsync();
		return NoContent();
	}
}
