using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using Util.Primitives.ResultType;
using WebXeDap.Domain.Models;
using WebXeDap.Infrastructure.Options;

namespace WebXeDap.Infrastructure.Services;

public sealed class EmailService : IEmailSender<User>
{
	private readonly EmailOptions options;

	public EmailService(EmailOptions options)
	{
		this.options = options;
	}

	public async Task<Result> SendEmailAsync(string to, string name, string subject, string body)
	{
		var msg = new MimeMessage();
		msg.From.Add(new MailboxAddress(options.FromName, options.FromEmail));
		msg.To.Add(new MailboxAddress(name, to));
		msg.Subject = subject;
		msg.Body = new TextPart("plain") { Text = body };

		using var smtp = new SmtpClient();
		var connectRes = await smtp.ConnectAsync(options.Host, options.Port, options.UseSSL)
			.ToResult(ex => new UnknownError($"Failed to connect to SMTP server: {ex.Message}"));
		if (connectRes.IsErr)
		{
			return connectRes.AsError;
		}
		var sendRes = await smtp.SendAsync(msg)
			.ToResult(ex => new UnknownError($"Failed to send email: {ex.Message}"));
		if (sendRes.IsErr)
		{
			return sendRes.AsError;
		}
		await smtp.DisconnectAsync(true);

		return Result.Ok();
	}

	public async Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
	{
		var name = user.FullName ?? "Unknown User";
		var res = await SendEmailAsync(
			email,
			name,
			"Confirm your email",
			$"Confirm your email: {confirmationLink}"
		);
		if (res.IsErr)
		{
			var error = res.AsError;
			Console.WriteLine($"Failed to send confirmation email to {email}: {error}");
		}
	}

	public async Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
	{
		var name = user.FullName ?? "Unknown User";
		var res = await SendEmailAsync(
			email,
			name,
			"Reset your password",
			$"Reset your password: {resetLink}"
		);
		if (res.IsErr)
		{
			var error = res.AsError;
			Console.WriteLine($"Failed to send password reset email to {email}: {error}");
		}
	}

	public async Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
	{
		var name = user.FullName ?? "Unknown User";
		var res = await SendEmailAsync(
			email,
			name,
			"Your password reset code",
			$"Your password reset code is: {resetCode}"
		);
		if (res.IsErr)
		{
			var error = res.AsError;
			Console.WriteLine($"Failed to send password reset code email to {email}: {error}");
		}
	}
}
