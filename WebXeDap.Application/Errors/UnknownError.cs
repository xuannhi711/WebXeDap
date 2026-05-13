using Util.Primitives.ResultType;

namespace WebXeDap.Application.Errors;

public record UnknownError(string Message) : Error;