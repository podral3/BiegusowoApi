using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace BiegusowoApi.Helpers;

public enum ServiceError { NotFound, Forbidden, Conflict, ValidationError }

public readonly struct Result
{
    public ServiceError? Error { get; }
    public bool IsSuccess => Error is null;

    private Result(ServiceError? error) => Error = error;

    public static Result Success() => new(null);
    public static Result Failure(ServiceError error) => new(error);
}

public readonly struct Result<T>
{
    public T? Value { get; }
    public ServiceError? Error { get; }
    public int? SuccessStatusCode { get; }
    public bool IsSuccess => Error is null;

    private Result(T value, int? successStatusCode) =>
        (Value, Error, SuccessStatusCode) = (value, null, successStatusCode);

    private Result(ServiceError error) => (Value, Error, SuccessStatusCode) = (default, error, null);

    public static Result<T> Success(T value) => new(value, null);
    public static Result<T> Success(T value, int statusCode) => new(value, statusCode);
    public static Result<T> Failure(ServiceError error) => new(error);
}


public static class ResultExtensions
{
    public static ActionResult<T> ToActionResult<T>(this Result<T> result, ControllerBase controller)
    {
        if (result.IsSuccess)
        {
            return result.SuccessStatusCode switch
            {
                null or StatusCodes.Status200OK => controller.Ok(result.Value),
                StatusCodes.Status201Created => controller.StatusCode(StatusCodes.Status201Created, result.Value),
                StatusCodes.Status204NoContent => controller.NoContent(),
                var code => controller.StatusCode(code.Value, result.Value)
            };
        }

        return result.Error switch
        {
            ServiceError.NotFound => controller.NotFound(),
            ServiceError.Forbidden => controller.StatusCode(StatusCodes.Status403Forbidden),
            ServiceError.Conflict => controller.StatusCode(StatusCodes.Status409Conflict),
            ServiceError.ValidationError => controller.BadRequest(),
            _ => controller.StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    public static ActionResult ToActionResult(this Result result, ControllerBase controller)
    {
        if (result.IsSuccess)
            return controller.NoContent();

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


