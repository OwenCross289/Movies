using Movies.Contracts.Responses;
using ValidationException = FluentValidation.ValidationException;

namespace Movies.Api.Mapping;

public class ValidationMappingMiddleware
{
    private readonly RequestDelegate _next;

    public ValidationMappingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException e)
        {
            context.Response.StatusCode = 400;
            var validationFailureResponse =
                new ValidationFailureResponse(e.Errors.Select(x =>
                    new ValidationResponse(x.PropertyName, x.ErrorMessage)));

            await context.Response.WriteAsJsonAsync(validationFailureResponse);
        }
    }
}