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

	[HttpGet("google-login")]
	public IActionResult GoogleLogin()
	{
		var redirectUrl = Url.Action(nameof(GoogleCallback));

		var properties = _signInManager.ConfigureExternalAuthenticationProperties(
			"Google",
			redirectUrl
		);

		return Challenge(properties, "Google");
	}

	[HttpGet("signin-google")]
	public async Task<IActionResult> GoogleCallback()
	{
		var info = await _signInManager.GetExternalLoginInfoAsync();

		if (info == null)
			return BadRequest();

		var result = await _signInManager.ExternalLoginSignInAsync(
			info.LoginProvider,
			info.ProviderKey,
			false
		);

		if (!result.Succeeded)
		{
			var email = info.Principal.FindFirstValue(ClaimTypes.Email);

			var user = new User
			{
				UserName = email,
				Email = email,
				EmailConfirmed = true,
			};

			await _userManager.CreateAsync(user);

			await _userManager.AddLoginAsync(user, info);

			await _signInManager.SignInAsync(user, false);
		}

		return Ok("Logged in");
	}
}
