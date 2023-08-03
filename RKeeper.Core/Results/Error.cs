namespace RKeeper.Core.Results;

public interface IError
{
    public string Message { get; }
    public object? Data { get; }
    public ErrorClass Type { get; }
}

public class Error : IError
{
    public string Message { get; }
    public object? Data { get; }
    public ErrorClass Type { get; }

    internal Error(ErrorClass type, string message, object? data = null)
    {
        Type = type;
        Message = message;
        Data = data;
    }

    public static Error Create(ErrorClass type, string message, object? data = null)
    {
        return new Error(type, message, data);
    }

    public static Error ValidationError(string message, object? data = null)
    {
        return new Error(ErrorClass.ValidationError, message, data);
    }

    public static Error NotFoundError(string message, object? data = null)
    {
        return new Error(ErrorClass.NotFoundError, message, data);
    }

    public static Error InvalidOperationError(string message, object? data = null)
    {
        return new Error(ErrorClass.InvalidOperationError, message, data);
    }
}
