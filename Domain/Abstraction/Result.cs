using Domain.Exception;

namespace Domain.Abstraction;

public sealed class Result<T>
{
    private readonly T? value;
    private readonly System.Exception? error;

    public Result(T value)
    {
        this.IsError = false;
        this.value = value;
        this.error = default;
    }

    private Result(System.Exception error)
    {
        this.IsError = true;
        this.value = default;
        this.error = error;
    }

    public bool IsError { get; private set; }

    public bool IsSuccess => !this.IsError;

    public System.Exception? Error => this.error;

    public static implicit operator Result<T>(T value) => new(value);

    public static implicit operator Result<T>(System.Exception error) => new(error);

    public TResult Match<TResult>(
        Func<T, TResult> success,
        Func<System.Exception, TResult> failure) =>
        !this.IsError ? success(this.value!) : failure(this.error!);

    /// <summary>
    /// Assumes result is successful and returns value.
    /// If result is not successful will throw a UnhandledResultException with the failure exception as InnerException.
    /// </summary>
    /// <exception cref="UnhandledResultException">Thrown if result is failure.</exception>
    /// <returns>T.</returns>
    public T Unwrap()
    {
        return this.Match(
            (T value) => value,
            (System.Exception error) => throw new UnhandledResultException(error));
    }
}