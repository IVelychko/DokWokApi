namespace DokWokApi.DAL.ResultType;

public readonly struct Result<TValue>
{
    private readonly ResultState _state;

    private readonly TValue? _value;

    private readonly Exception? _exception;

    public Result(TValue value)
    {
        _state = ResultState.Success;
        _value = value;
        _exception = null;
    }

    public Result(Exception exception)
    {
        _state = ResultState.Faulted;
        _exception = exception;
        _value = default;
    }

    public bool IsSuccess => _state == ResultState.Success;

    public bool IsFaulted => !IsSuccess;

    public TValue Value => IsSuccess ? _value! 
        : throw new InvalidOperationException("It is not possible to access the value if the result state is faulted");

    public Exception Exception => IsFaulted ? _exception!
        : throw new InvalidOperationException("It is not possible to access the exception if the result state is success");

    public static implicit operator Result<TValue>(TValue value) => new(value);

    public TResult Match<TResult>(Func<TValue, TResult> success, Func<Exception, TResult> failure) =>
        IsSuccess ? success(_value!) : failure(_exception!);
}
