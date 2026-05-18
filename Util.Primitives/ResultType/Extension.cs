namespace Util.Primitives.ResultType;

public static class ResultExtensions
{
	public static async Task<Result<T>> ToResult<T>(
		this Task<T> task,
		Func<Exception, Error> errorHandler
	)
	{
		try
		{
			var result = await task;
			return result;
		}
		catch (Exception ex)
		{
			return errorHandler(ex);
		}
	}

	public static Result<T> ToResult<T>(this Func<T> func, Func<Exception, Error> errorHandler)
	{
		try
		{
			var result = func();
			return result;
		}
		catch (Exception ex)
		{
			return errorHandler(ex);
		}
	}

	public static async Task<Result> ToResult(this Task task, Func<Exception, Error> errorHandler)
	{
		try
		{
			await task;
			return Result.Ok();
		}
		catch (Exception ex)
		{
			return errorHandler(ex);
		}
	}
}
