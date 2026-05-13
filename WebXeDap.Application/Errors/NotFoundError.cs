using Util.Primitives.ResultType;

namespace WebXeDap.Application.Errors;

public record NotFoundError(string Message) : Error;