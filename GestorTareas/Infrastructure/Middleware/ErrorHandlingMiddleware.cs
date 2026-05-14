using System.Net;
using System.Text.Json;

namespace GestorTareas.Infrastructure.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                // 404
                if (context.Response.StatusCode == (int)HttpStatusCode.NotFound)
                {
                    await EscribirProblemDetails(context, StatusCodes.Status404NotFound,
                        "Recurso no encontrado", "El recurso solicitado no existe.");
                }
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogWarning(ex, "Solicitud mal formada: {Message}", ex.Message);
                await EscribirProblemDetails(context, StatusCodes.Status400BadRequest,
                    "Solicitud inválida", ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argumento inválido: {Message}", ex.Message);
                await EscribirProblemDetails(context, StatusCodes.Status400BadRequest,
                    "Argumento inválido", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno del servidor: {Message}", ex.Message);
                await EscribirProblemDetails(context, StatusCodes.Status500InternalServerError,
                    "Error interno del servidor", "Ha ocurrido un error inesperado.");
            }
        }

        private static async Task EscribirProblemDetails(HttpContext context, int statusCode, string title, string detail)
        {
            if (context.Response.HasStarted)
                return;

            context.Response.Clear();
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = statusCode;

            var problemDetails = new
            {
                type = $"https://httpstatuses.io/{statusCode}",
                title,
                status = statusCode,
                detail,
                instance = context.Request.Path
            };

            var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }

    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}