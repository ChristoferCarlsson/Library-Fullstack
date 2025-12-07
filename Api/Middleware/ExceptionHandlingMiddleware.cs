using System.Net;
using System.Text.Json;
using Application.Exceptions;

namespace Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
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

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            string errorCode = "internal_error";

            switch (ex)
            {
                case NotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    errorCode = "not_found";
                    break;

                case ValidationException:
                    statusCode = HttpStatusCode.BadRequest;
                    errorCode = "validation_error";
                    break;
            }

            _logger.LogError(ex, "Unhandled exception occurred.");

            var problem = new
            {
                status = (int)statusCode,
                code = errorCode,
                message = ex.Message,
                traceId = context.TraceIdentifier
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}
