using Microsoft.AspNetCore.Mvc;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.WebAPI.Services;

namespace WebXeDap.WebAPI.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
	private const string RefreshTokenCookieName = "refreshToken";
	private readonly UserService _userService;

	public AuthController(UserService userService)
	{
		_userService = userService;
	}

	[HttpPost("login")]
	public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
	{
		if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
		{
			return BadRequest("Email and password are required.");
		}

		var result = await _userService.LoginAsync(request.Email, request.Password);
		if (!result.IsOk)
		{
			return BadRequest(result.Error);
		}
		var tokens = result.Value;

		SetRefreshTokenCookie(tokens.RefreshToken, tokens.RefreshTokenExpiresAtUtc);
		return Ok(new LoginResponse(tokens.AccessToken, tokens.RefreshToken));
	}

	[HttpPost("register")]
	public async Task<ActionResult> Register([FromBody] RegisterRequest request)
	{
		if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
		{
			return BadRequest("Email and password are required.");
		}

		var result = await _userService.RegisterAsync(request.Email, request.Password);
		if (!result.IsOk)
		{
			return BadRequest(result.Error);
		}

		return Ok();
	}

	[HttpPost("confirm-email")]
	public async Task<ActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
	{
		if (request.UserId <= 0 || string.IsNullOrWhiteSpace(request.Token))
		{
			return BadRequest("UserId and token are required.");
		}

		var result = await _userService.ConfirmEmailAsync(request.UserId, request.Token);
		if (!result.IsOk)
		{
			return BadRequest(result.Error);
		}

		return Ok();
	}

	private void SetRefreshTokenCookie(string refreshToken, DateTime expiresAtUtc)
	{
		var options = new CookieOptions
		{
			HttpOnly = true,
			Secure = Request.IsHttps,
			SameSite = SameSiteMode.Lax,
			Expires = expiresAtUtc,
		};

		Response.Cookies.Append(RefreshTokenCookieName, refreshToken, options);
	}

	public sealed record LoginRequest(string Email, string Password);

	public sealed record LoginResponse(string AccessToken, string RefreshToken);

	public sealed record RegisterRequest(string Email, string Password);

	public sealed record ConfirmEmailRequest(int UserId, string Token);
}
