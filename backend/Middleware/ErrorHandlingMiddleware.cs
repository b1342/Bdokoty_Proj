using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WorkshowcaseApi.Common.Exceptions;

namespace WorkshowcaseApi.Middleware;

public sealed class ErrorHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        if (exception is AppException appException)
        {
            context.Response.StatusCode = appException.StatusCode;

            if (TryGetValidationErrors(exception, out var errors))
            {
                var payloadWithErrors = new
                {
                    message = appException.Message,
                    errors
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(payloadWithErrors, JsonOptions));
                return;
            }

            var payload = new { message = appException.Message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOptions));
            return;
        }

        if (exception is FluentValidation.ValidationException fluentValidationException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var errors = fluentValidationException.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(y => y.ErrorMessage).Distinct().ToArray());

            var payload = new
            {
                message = "Validation failed.",
                errors
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOptions));
            return;
        }

        _logger.LogError(exception, "Unhandled exception occurred.");
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var genericPayload = new { message = "An unexpected error occurred." };
        await context.Response.WriteAsync(JsonSerializer.Serialize(genericPayload, JsonOptions));
    }

    private static bool TryGetValidationErrors(Exception exception, out IReadOnlyDictionary<string, string[]>? errors)
    {
        errors = null;

        var errorsProperty = exception.GetType().GetProperty("Errors");
        if (errorsProperty is null)
        {
            return false;
        }

        var value = errorsProperty.GetValue(exception);
        if (value is IReadOnlyDictionary<string, string[]> typedReadOnlyDictionary)
        {
            errors = typedReadOnlyDictionary;
            return true;
        }

        if (value is Dictionary<string, string[]> typedDictionary)
        {
            errors = typedDictionary;
            return true;
        }

        return false;
    }
}
