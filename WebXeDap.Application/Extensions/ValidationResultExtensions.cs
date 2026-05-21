using FluentValidation.Results;
using Util.Primitives.ResultType;

namespace WebXeDap.Application.Extensions;

public static class ValidationResultExtensions
{
	public static ValidationError ToValidationError(this ValidationResult validationResult)
	{
		return new ValidationError(validationResult.ToDictionary());
	}
}
