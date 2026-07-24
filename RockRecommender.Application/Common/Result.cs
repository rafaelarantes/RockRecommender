namespace RockRecommender.Application.Common;

public sealed class Result<T>
{
    private Result(bool isSuccess, T? value, string? error, ResultFailure? failure)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        Failure = failure;
    }

    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public ResultFailure? Failure { get; }

    public static Result<T> Success(T value) => new(true, value, null, null);
    public static Result<T> Invalid(string error) => new(false, default, error, ResultFailure.Invalid);
    public static Result<T> NotFound(string error) => new(false, default, error, ResultFailure.NotFound);
    public static Result<T> Conflict(string error) => new(false, default, error, ResultFailure.Conflict);
    public static Result<T> Unavailable(string error) => new(false, default, error, ResultFailure.Unavailable);
}
