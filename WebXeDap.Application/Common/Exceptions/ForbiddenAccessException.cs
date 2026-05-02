namespace WebXeDap.Application.Common.Exceptions;

public sealed class ForbiddenAccessException : Exception
{
	public ForbiddenAccessException()
		: base("Access is forbidden.")
	{
	}
}
