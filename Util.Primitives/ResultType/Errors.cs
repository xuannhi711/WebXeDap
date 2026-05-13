namespace Util.Primitives.ResultType;

public abstract record Error;

public record NotFoundError(string Message) : Error;

public record UnknownError(string Message) : Error;

public record ValidationError(Dictionary<string, string> Errors) : Error;
