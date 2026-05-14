using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using WebXeDap.Domain.Models;

namespace WebXeDap.WebAPI.Services;

public sealed class NoOpEmailSender : IEmailSender<User>
{
	private readonly ILogger<NoOpEmailSender> _logger;

	public NoOpEmailSender(ILogger<NoOpEmailSender> logger)
	{
		_logger = logger;
	}

	public Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
	{
		_logger.LogInformation("Email confirmation requested for {Email}.", email);
		return Task.CompletedTask;
	}

	public Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
	{
		_logger.LogInformation("Password reset link requested for {Email}.", email);
		return Task.CompletedTask;
	}

	public Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
	{
		_logger.LogInformation("Password reset code requested for {Email}.", email);
		return Task.CompletedTask;
	}
}
