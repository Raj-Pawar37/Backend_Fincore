using Backend_Fincore.DTOs;
using Backend_Fincore.Response;
using System.Net;
using System.Text.Json;

namespace Backend_Fincore.Middleware
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

        private async Task HandleExceptionAsync(HttpContext context,Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception occurred. Path: {Path}",context.Request.Path);

            int statusCode = exception switch
            {
                ArgumentException => StatusCodes.Status400BadRequest,

                KeyNotFoundException => StatusCodes.Status404NotFound,

                InvalidOperationException => StatusCodes.Status409Conflict,

                UnauthorizedAccessException =>StatusCodes.Status401Unauthorized,

                _ => StatusCodes.Status500InternalServerError
            };

            string message = statusCode == StatusCodes.Status500InternalServerError ? "An internal server error occurred." : exception.Message;

            var response = new ApiResponse<object>
            {
                Success = false,
                Message = message,
                Data = null,
                Error = exception.Message
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var json = JsonSerializer.Serialize(response,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

            await context.Response.WriteAsync(json);
        }
    }
}