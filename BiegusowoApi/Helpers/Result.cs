using Microsoft.AspNetCore.Mvc;

namespace BiegusowoApi.Helpers;

public enum ServiceError { NotFound, Forbidden, Conflict, ValidationError }

public readonly struct Result<T>
{
    public T? Value { get; }
    public ServiceError? Error { get; }
    public bool IsSuccess => Error is null;

    private Result(T value) => (Value, Error) = (value, null);
    private Result(ServiceError error) => (Value, Error) = (default, error);

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(ServiceError error) => new(error);
}

public static class ResultExtensions
{
    public static ActionResult<T> ToActionResult<T>(this Result<T> result, ControllerBase controller)
    {
        if (result.IsSuccess)
            return controller.Ok(result.Value);

        return result.Error switch
        {
            ServiceError.NotFound => controller.NotFound(),
            ServiceError.Forbidden => controller.StatusCode(StatusCodes.Status403Forbidden),
            ServiceError.Conflict => controller.StatusCode(StatusCodes.Status409Conflict),
            ServiceError.ValidationError => controller.BadRequest(),
            _ => controller.StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}