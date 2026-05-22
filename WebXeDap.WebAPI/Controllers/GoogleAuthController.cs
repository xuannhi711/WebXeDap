using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebXeDap.Domain.Models;

namespace WebXeDap.WebAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class GoogleAuthController : ControllerBase
{
	private readonly SignInManager<User> _signInManager;
	private readonly UserManager<User> _userManager;

	public GoogleAuthController(SignInManager<User> signInManager, UserManager<User> userManager)
	{
		_signInManager = signInManager;
		_userManager = userManager;
	}

	[HttpGet("login/google")]
	public IActionResult GoogleLogin([FromQuery] string? origin = null)
	{
		var redirectUrl = Url.Action(nameof(GoogleCallback), values: new { origin });
		var properties = _signInManager.ConfigureExternalAuthenticationProperties(
			"Google",
			redirectUrl
		);

		return Challenge(properties, "Google");
	}

	[HttpGet("google-callback")]
	public async Task<IActionResult> GoogleCallback([FromQuery] string? origin = null)
	{
		var info = await _signInManager.GetExternalLoginInfoAsync();

		if (info == null)
			return BadRequest();

		var result = await _signInManager.ExternalLoginSignInAsync(
			info.LoginProvider,
			info.ProviderKey,
			true
		);
		if (result.Succeeded)
		{
			return Redirect(origin ?? "/");
		}

		var email = info.Principal.FindFirstValue(ClaimTypes.Email);
		var avatar = info.Principal.FindFirstValue("picture");
		if (string.IsNullOrWhiteSpace(avatar))
		{
			return BadRequest("Google account does not have a profile picture.");
		}
		var name = info.Principal.FindFirstValue(ClaimTypes.Name);

		var user = new User
		{
			FullName = name,
			UserName = email,
			Email = email,
			EmailConfirmed = true,
			Avatar = avatar,
		};

		await _userManager.CreateAsync(user);

		await _userManager.AddLoginAsync(user, info);

		await _signInManager.SignInAsync(user, true);

		return Redirect(origin ?? "/");
	}
}
