using Util.Primitives.ResultType;

namespace Util.Primitives.UnitTests;

[Trait("Category", "Unit")]
public class ResultExtensionsTests
{
	[Fact]
	public async Task ToResultAsync_SuccessTest()
	{
		var task = Task.FromResult(42);
		var res = await task.ToResult(ex => new UnknownError(ex.Message));
		Assert.True(res.IsOk);
		Assert.False(res.IsErr);
		Assert.Equal(42, res.Value);
	}

	[Fact]
	public async Task ToResultAsync_ExceptionTest()
	{
		var task = Task.FromException<int>(new InvalidOperationException("Invalid operation"));
		var res = await task.ToResult(ex =>
			ex switch
			{
				InvalidOperationException => new NotFoundError("Resource not found."),
				_ => new UnknownError(ex.Message),
			}
		);
		Assert.False(res.IsOk);
		Assert.True(res.IsErr);
		var err = Assert.IsType<NotFoundError>(res.Value);
		Assert.Equal("Resource not found.", err.Message);
	}

	[Fact]
	public void ToResult_SuccessTest()
	{
		var func = new Func<string>(() => "Hello, World!");
		var res = func.ToResult(ex => new UnknownError(ex.Message));
		Assert.True(res.IsOk);
		Assert.False(res.IsErr);
		Assert.Equal("Hello, World!", res.Value);
	}

	[Fact]
	public void ToResult_ExceptionTest()
	{
		var func = new Func<string>(() =>
			throw new ArgumentNullException("param", "Parameter cannot be null")
		);
		var res = func.ToResult(ex =>
			ex switch
			{
				ArgumentNullException => new NotFoundError("Resource not found."),
				_ => new UnknownError(ex.Message),
			}
		);
		Assert.False(res.IsOk);
		Assert.True(res.IsErr);
		var err = Assert.IsType<NotFoundError>(res.Value);
		Assert.Equal("Resource not found.", err.Message);
	}

	[Fact]
	public void ToResult_UnexpectedExceptionTest()
	{
		var func = new Func<string>(() => throw new Exception("Unexpected error"));
		var res = func.ToResult(ex =>
			ex switch
			{
				ArgumentNullException => new NotFoundError("Resource not found."),
				_ => new UnknownError(ex.Message),
			}
		);
		Assert.False(res.IsOk);
		Assert.True(res.IsErr);
		var err = Assert.IsType<UnknownError>(res.Value);
		Assert.Equal("Unexpected error", err.Message);
	}

	[Fact]
	public async Task ToResultAsync_UnexpectedExceptionTest()
	{
		var task = Task.FromException<int>(new Exception("Unexpected error"));
		var res = await task.ToResult(ex =>
			ex switch
			{
				ArgumentNullException => new NotFoundError("Resource not found."),
				_ => new UnknownError(ex.Message),
			}
		);
		Assert.False(res.IsOk);
		Assert.True(res.IsErr);
		var err = Assert.IsType<UnknownError>(res.Value);
		Assert.Equal("Unexpected error", err.Message);
	}

	[Fact]
	public void ToResult_NestedExceptionTest()
	{
		var func = new Func<string>(() =>
			throw new Exception("Outer exception", new InvalidOperationException("Inner exception"))
		);
		var res = func.ToResult(ex =>
			ex switch
			{
				ArgumentNullException => new NotFoundError("Resource not found."),
				InvalidOperationException => new NotFoundError("Invalid operation."),
				_ => new UnknownError(ex.Message),
			}
		);
		Assert.False(res.IsOk);
		Assert.True(res.IsErr);
		var err = Assert.IsType<UnknownError>(res.Value);
		Assert.Equal("Outer exception", err.Message);
	}
}
