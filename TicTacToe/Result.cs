namespace TicTacToe;

public readonly record struct Success<TValue>(TValue Value);

public readonly record struct Failure<TError>(TError Error);

public static class Result
{
    public static Success<TValue> Success<TValue>(TValue value)
        => new(value);

    public static Failure<TError> Failure<TError>(TError error)
        => new(error);
    
}

public readonly record struct Result<TValue, TError>
{
    private readonly TValue? value;
    private readonly TError? error;
    
    public bool IsSuccess { get; }

    private Result(
        TValue? value,
        TError? error,
        bool isSuccess)
    {
        this.value = value;
        this.error = error;
        IsSuccess = isSuccess;
    }

    public static implicit operator Result<TValue, TError>(
        Success<TValue> success)
        => new(success.Value, default, true);

    public static implicit operator Result<TValue, TError>(
        Failure<TError> failure)
        => new(default, failure.Error, false);
    
    public TResult Match<TResult>(
        Func<TValue, TResult> onSuccess,
        Func<TError, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(value!) : onFailure(error!);
    }
}

