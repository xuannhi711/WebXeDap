namespace WebXeDap.Application.Contracts.Infrastructure;

public interface IEmailService
{
	Task<bool> SendEmail(Email email);
};

public record Email(string To, string Subject, string Body);
