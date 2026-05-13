using Util.Primitives.ResultType;

namespace WebXeDap.Application.Errors;

public record ValidationError(Dictionary<string, string> Errors) : Error;