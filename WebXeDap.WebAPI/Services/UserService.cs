using Microsoft.AspNetCore.Identity;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Errors;
using Util.Primitives.ResultType;
using WebXeDap.Domain.Models;

namespace WebXeDap.WebAPI.Services;

public sealed class UserService : IUserService
{
	private readonly UserManager<User> _userManager;
	private readonly TokenService _tokenService;

	public UserService(UserManager<User> userManager, TokenService tokenService)
	{
		_userManager = userManager;
		_tokenService = tokenService;
	}

	public async Task<Result<TokensResult>> LoginAsync(string email, string password)
	{
		var user = await _userManager.FindByEmailAsync(email);
		if (user is null)
		{
			var errors = new Dictionary<string, string> { { "email", "Email not found." } };
			return new ValidationError(errors);
		}

		var validPassword = await _userManager.CheckPasswordAsync(user, password);
		if (!validPassword)
		{
			var errors = new Dictionary<string, string> { { "password", "Invalid password." } };
			return new ValidationError(errors);
		}

		var roles = await _userManager.GetRolesAsync(user);
		return _tokenService.CreateTokenPair(user, roles);
	}

	public async Task<Result> RegisterAsync(string email, string password)
	{
		var user = new User
		{
			UserName = email.ToLowerInvariant(),
			Email = email.ToLowerInvariant(),
		};

		var createResult = await _userManager.CreateAsync(user, password);
		if (!createResult.Succeeded)
		{
			var errors = createResult.Errors.ToDictionary(e => e.Code, e => e.Description);
			return new ValidationError(errors);
		}

		var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
		return Result.Ok();
	}

	public async Task<Result> ConfirmEmailAsync(int userID, string token)
	{
		var user = await _userManager.FindByIdAsync(userID.ToString());
		if (user is null)
		{
			return new NotFoundError("User not found.");
		}

		var result = await _userManager.ConfirmEmailAsync(user, token);
		if (!result.Succeeded)
		{
			var errors = result.Errors.ToDictionary(e => e.Code, e => e.Description);
			return new ValidationError(errors);
		}

		return Result.Ok();
	}
}
