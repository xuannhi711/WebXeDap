using Util.Primitives.ResultType;

namespace Util.Primitives.Tests;

public class CustomResultTypeTests
{
	[Fact]
	public void CustomResultType_IsOkAndIsErrTest()
	{
		Result<string> res1 = "test";
		Assert.True(res1.IsOk);
		Assert.False(res1.IsErr);

		Result<int> res2 = 42;
		Assert.True(res2.IsOk);
		Assert.False(res2.IsErr);

		Result<List<string>> res3 = new List<string> { "a", "b", "c" };
		Assert.True(res3.IsOk);
		Assert.False(res3.IsErr);

		Result res4 = new NotFoundError("Not found");
		Assert.False(res4.IsOk);
		Assert.True(res4.IsErr);

		Result<string> res5 = new ValidationError(
			new Dictionary<string, string[]> { { "email", ["Invalid email format"] } }
		);
		Assert.False(res5.IsOk);
		Assert.True(res5.IsErr);
	}

	[Fact]
	public void CustomResultType_TryPickValueTest()
	{
		// 1. NotFoundError
		Result<int> res1 = new NotFoundError("Not found");
		var isOk1 = res1.TryPickValue(out _, out var err1);
		Assert.False(isOk1);
		var notFoundErr = Assert.IsType<NotFoundError>(err1);
		Assert.Equal("Not found", notFoundErr.Message);

		// 2. ValidationError
		Result<int> res2 = new ValidationError(
			new Dictionary<string, string[]>
			{
				{ "email", ["Invalid email format"] },
				{ "password", ["Password is too short"] },
			}
		);
		var isOk2 = res2.TryPickValue(out _, out var err2);
		Assert.False(isOk2);
		var validationErr2 = Assert.IsType<ValidationError>(err2);
		Assert.Equal(2, validationErr2.Errors.Count);

		Assert.Equal(["Invalid email format"], validationErr2.Errors["email"]);
		Assert.Equal(["Password is too short"], validationErr2.Errors["password"]);

		// 3. Guid
		var guid3 = Guid.NewGuid();
		Result<Guid> res3 = guid3;
		var isOk3 = res3.TryPickValue(out var value3);
		Assert.True(isOk3);
		Assert.Equal(guid3, value3);

		// 4. null
		Result<string?> res4 = new NotFoundError("Not found");
		var isOk4 = res4.TryPickValue(out var value4);
		Assert.False(isOk4);
		Assert.Null(value4);

		// 5. complex object
		var product5 = new TestProduct { Id = 1, Name = "Bike" };
		Result<TestProduct> res5 = product5;
		var isOk5 = res5.TryPickValue(out var value5);
		Assert.True(isOk5);
		Assert.Same(product5, value5);
		Assert.Equal(1, value5.Id);
		Assert.Equal("Bike", value5.Name);
	}

	[Fact]
	public void CustomResultType_TryPickErrorTest()
	{
		// 1. NotFoundError
		Result<int> res1 = new NotFoundError("Not found");
		var isErr1 = res1.TryPickError(out var err1);
		Assert.True(isErr1);
		var notFoundErr = Assert.IsType<NotFoundError>(err1);
		Assert.Equal("Not found", notFoundErr.Message);

		// 2. ValidationError
		Result<int> res2 = new ValidationError(
			new Dictionary<string, string[]>
			{
				{ "email", ["Invalid email format"] },
				{ "password", ["Password is too short"] },
			}
		);
		var isErr2 = res2.TryPickError(out var err2);
		Assert.True(isErr2);
		var validationErr2 = Assert.IsType<ValidationError>(err2);
		Assert.Equal(2, validationErr2.Errors.Count);
		Assert.Equal(["Invalid email format"], validationErr2.Errors["email"]);
		Assert.Equal(["Password is too short"], validationErr2.Errors["password"]);

		// 3. Guid
		var guid3 = Guid.NewGuid();
		Result<Guid> res3 = guid3;
		var isErr3 = res3.TryPickError(out var err3);
		Assert.False(isErr3);
		Assert.Null(err3);
	}

	[Fact]
	public void CustomResultType_ValueWhenErrTest()
	{
		// 1
		Result<int> result1 = new NotFoundError("Not found");
		Assert.True(result1.IsErr);
		var err1 = Assert.IsType<NotFoundError>(result1.Value);
		Assert.Equal("Not found", err1.Message);

		// 2
		Result<string> result2 = new ValidationError(
			new Dictionary<string, string[]>
			{
				{ "email", ["Invalid email format"] },
				{ "password", ["Password is too short"] },
			}
		);
		Assert.True(result2.IsErr);
		var err2 = Assert.IsType<ValidationError>(result2.Value);
		Assert.Equal(2, err2.Errors.Count);
		Assert.Equal(["Invalid email format"], err2.Errors["email"]);
		Assert.Equal(["Password is too short"], err2.Errors["password"]);

		// 3
		var err3 = new NotFoundError("Not found");
		Result<string> result3 = err3;
		result3.Switch(
			value => throw new Exception("Expected Err, but got Ok"),
			error =>
			{
				var e = error switch
				{
					NotFoundError nf => nf,
					ValidationError ve => throw new Exception(
						"Expected NotFoundError, but got ValidationError"
					),
					_ => throw new Exception("Unexpected error type"),
				};
				Assert.Same(err3, e);
			}
		);
	}

	[Fact]
	public void CustomResultType_ValueWhenOkTest()
	{
		Result<string> result1 = "Hello, World!";
		Assert.True(result1.IsOk);
		Assert.Equal("Hello, World!", result1.Value);

		Result<int> result2 = 123;
		Assert.True(result2.IsOk);
		Assert.Equal(123, result2.Value);

		var product = new TestProduct { Id = 1, Name = "Bike" };
		Result<TestProduct> result3 = product;
		Assert.True(result3.IsOk);
		var value3 = result3.Match(
			val => val,
			err => throw new Exception("Expected Ok, but got Err")
		);
		Assert.Same(product, value3);
		Assert.Equal(1, value3.Id);
		Assert.Equal("Bike", value3.Name);
	}

	private sealed class TestProduct
	{
		public int Id { get; init; }

		public required string Name { get; init; }
	}
}
