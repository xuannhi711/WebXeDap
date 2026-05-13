using System.Diagnostics.CodeAnalysis;
using OneOf;
using OneOf.Types;

namespace Util.Primitives.ResultType;

public abstract record Error;

public class Result<T> : OneOfBase<T, Error>
{
	protected Result(OneOf<T, Error> _)
		: base(_) { }

	public bool IsOk => IsT0;
	public bool IsErr => IsT1;

	public bool TryPickValue(out T value, out Error error) =>
		TryPickT0(out value, out error);

	public bool TryPickValue(out T value) =>
	TryPickT0(out value, out _);

	public bool TryPickError(out Error error, out T value) =>
		TryPickT1(out error, out value);

	public bool TryPickError(out Error error) =>
	TryPickT1(out error, out _);

	public static implicit operator Result<T>(T value) => new(value);

	public static implicit operator Result<T>(Error error) => new(error);

	public static Result<T> Ok(T value) => new(value);
	public static Result<T> Err(Error error) => new(error);
}

public class Result : Result<None>
{
	private Result(OneOf<None, Error> _)
		: base(_) { }

	public static Result Ok() => new(new None());

	public static implicit operator Result(Error error) => new(error);

}