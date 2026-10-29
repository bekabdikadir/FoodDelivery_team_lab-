namespace FoodDelivery.BLL.Models;

public class ServiceResult
{
    public bool Succeeded { get; protected init; }
    public IReadOnlyList<string> Errors { get; protected init; } = Array.Empty<string>();

    public static ServiceResult Success() => new() { Succeeded = true };

    public static ServiceResult Failure(params string[] errors) => new() { Errors = errors };
}

public class ServiceResult<T> : ServiceResult
{
    public T? Value { get; private init; }

    public static ServiceResult<T> Success(T value) => new() { Succeeded = true, Value = value };

    public new static ServiceResult<T> Failure(params string[] errors) => new() { Errors = errors };
}
