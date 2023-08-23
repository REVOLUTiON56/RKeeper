using System;
using System.Collections.Generic;
using System.Linq;

namespace RKeeper.Core.Results;

/* ----------------------------------------------------------------------------------
 * Reduced implementation structure which describes the result of the operation.
 * Possible to use on entire project to avoid Exceptions, because Exception means "Critical Error".
 * Also exception decrease performance.
 * Can be extended to containing multiple errors and combination among themselves
 * -------------------------------------------------------------------------------- */
public readonly struct Result
{
    public bool IsSuccess => IsFailure == false;
    public bool IsFailure { get; }
    public IReadOnlyCollection<IError>? Errors { get; }

    internal Result(bool isFailure, IEnumerable<IError>? errors)
    {
        IsFailure = isFailure;
        Errors = errors?.ToArray();
    }

    public static Result Ok()
    {
        return new Result(false, null);
    }

    public static Result Fail(IError error)
    {
        return new Result(true, new List<IError>(1) { error });
    }

    public static Result Fail(IEnumerable<IError> errors)
    {
        return new Result(true, errors);
    }

    public static Result Combine(Result result1, Result result2)
    {
        var errors = new List<IError>();

        if (result1.IsFailure)
        {
            errors.AddRange(result1.Errors!);
        }

        if (result2.IsFailure)
        {
            errors.AddRange(result2.Errors!);
        }

        return errors.Count > 0
            ? Fail(errors)
            : Ok();
    }

    public static Result<T> Ok<T>(T value)
    {
        return new Result<T>(false, value, null);
    }

    public static Result<T> Fail<T>(IError error)
    {
        return new Result<T>(true, default, new List<IError>(1) { error });
    }

    public static Result<T> Fail<T>(IEnumerable<IError> errors)
    {
        return new Result<T>(true, default, errors);
    }

    public void ThrowIfFailure()
    {
        if (IsSuccess)
        {
            return;
        }

        string error = Errors?.FirstOrDefault()?.Message ?? "Invalid operation";
        throw new InvalidOperationException(error);
    }
}

public readonly struct Result<T>
{
    public bool IsSuccess => IsFailure == false;
    public bool IsFailure { get; }
    public IReadOnlyCollection<IError>? Errors { get; }

    private readonly T? _value;
    public T? Value => IsSuccess ? _value : throw new InvalidOperationException("no value for failure result");

    internal Result(bool isFailure, T? value, IEnumerable<IError>? errors)
    {
        IsFailure = isFailure;
        Errors = errors?.ToArray();
        _value = value;
    }

    public void ThrowIfFailure()
    {
        if (IsSuccess)
        {
            return;
        }

        string error = Errors?.FirstOrDefault()?.Message ?? "Invalid operation";

        throw new InvalidOperationException(error);
    }

    public Result ToResult()
    {
        return new Result(IsFailure, Errors);
    }

    public Result<TR> As<TR>()
    {
        return new Result<TR>(IsFailure, default, Errors);
    }

    public static implicit operator Result(Result<T> result)
    {
        return result.ToResult();
    }

    public static implicit operator Result<T>(T value) => Result.Ok(value);
}
