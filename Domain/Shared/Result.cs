using Domain.Errors.Base;

namespace Domain.Shared;

public readonly struct Result<TValue>
{
    private readonly ResultState _state;

    private readonly TValue? _value;

    private readonly Error? _error;

    public Result(TValue value)
    {
        _state = ResultState.Success;
        _value = value;
        _error = null;
    }

    public Result(Error error)
    {
        _state = ResultState.Faulted;
        _error = error;
        _value = default;
    }

    public bool IsSuccess => _state == ResultState.Success;

    public bool IsFaulted => !IsSuccess;

    public TValue Value => IsSuccess ? _value!
        : throw new InvalidOperationException("It is not possible to access the value if the result state is faulted");

    public Error Error => IsFaulted ? _error!
        : throw new InvalidOperationException("It is not possible to access the error if the result state is success");

    public static implicit operator Result<TValue>(TValue value) => new(value);

    public static Result<TValue> Failure(Error error) => new(error);

    public TResult Match<TResult>(Func<TValue, TResult> success, Func<Error, TResult> failure) =>
        IsSuccess ? success(_value!) : failure(_error!);
}
