using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.Domain.Models;
using WebXeDap.WebAPI.Extensions;

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
	public async Task<ActionResult<User>> Me()
	{
		var user = await userManager.GetUserAsync(User);
		return Ok(user);
	}
}
