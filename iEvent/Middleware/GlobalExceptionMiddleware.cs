using iEvent.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace iEvent.WebApi.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, title, detail, errors) = exception switch
            {
                NotFoundException ex => (StatusCodes.Status404NotFound, "Not Found", ex.Message, Array.Empty<string>()),
                ConflictException ex => (StatusCodes.Status409Conflict, "Conflict", ex.Message, Array.Empty<string>()),
                UnauthorizedException ex => (StatusCodes.Status401Unauthorized, "Unauthorized", ex.Message, Array.Empty<string>()),
                ValidationException ex => (StatusCodes.Status400BadRequest, "Validation Failed", ex.Message, ex.Errors),
                ArgumentException ex => (StatusCodes.Status400BadRequest, "Validation Failed", ex.Message, Array.Empty<string>()),
                InvalidOperationException ex => (StatusCodes.Status400BadRequest, "Business Rule Violation", ex.Message, Array.Empty<string>()),
                _ => (StatusCodes.Status500InternalServerError, "Server Error", "An unexpected error occurred.", Array.Empty<string>())
            };

            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(exception, "Unhandled exception occurred.");
            }
            else
            {
                _logger.LogWarning(exception, "Request failed with status code {StatusCode}", statusCode);
            }

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };

            if (errors.Any())
            {
                problem.Extensions["errors"] = errors;
            }

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}