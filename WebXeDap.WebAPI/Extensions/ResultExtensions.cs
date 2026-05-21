using Microsoft.AspNetCore.Mvc;
using Util.Primitives.ResultType;

namespace WebXeDap.WebAPI.Extensions;

public static class ResultExtensions
{
	public static ActionResult<T> ToActionResult<T>(
		this ControllerBase controller,
		Result<T> result
	)
	{
		if (result.TryPickValue(out var value))
		{
			return controller.Ok(value);
		}

		return controller.MatchErrorResult(result.AsError);
	}

	public static IActionResult ToActionResult(this ControllerBase controller, Result result)
	{
		if (result.IsOk)
		{
			return controller.NoContent();
		}

		return controller.MatchErrorResult(result.AsError);
	}

	public static ActionResult MatchErrorResult(this ControllerBase controller, Error error)
	{
		return error switch
		{
			ValidationError validationError => controller.ValidationProblem(
				new ValidationProblemDetails(validationError.Errors)
			),
			NotFoundError notFoundError => controller.NotFound(
				new { message = notFoundError.Message }
			),
			UnauthorizedError => controller.Unauthorized(),
			ForbiddenError => controller.Forbid(),
			UnknownError unknownError => controller.Problem(
				detail: unknownError.Message,
				title: "An unknown error occurred."
			),
			_ => controller.Problem(detail: error.ToString()),
		};
	}
}
