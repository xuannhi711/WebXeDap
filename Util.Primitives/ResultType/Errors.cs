using System.Text.Json;

namespace Util.Primitives.ResultType;

public abstract record Error;

public record NotFoundError(string Message) : Error
{
	public override string ToString()
	{
		return $"Not found: {Message}";
	}
};

public record UnknownError(string Message) : Error
{
	public override string ToString()
	{
		return $"Unknown error: {Message}";
	}
};

public record ValidationError(IDictionary<string, string[]> Errors) : Error
{
	private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

	public override string ToString()
	{
		return $"Validation errors: {JsonSerializer.Serialize(Errors, JsonOptions)}";
	}
}

public record InternalError(string Message) : Error
{
	public override string ToString()
	{
		return $"Internal error: {Message}";
	}
}

public record ForbiddenError(string Message) : Error
{
	public override string ToString()
	{
		return $"Forbidden: {Message}";
	}
}

public record UnauthorizedError(string Message) : Error
{
	public override string ToString()
	{
		return $"Unauthorized: {Message}";
	}
}
