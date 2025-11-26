using System.Net;
using System.Text.Json;
using PedidoMestre.Models.Common;

namespace PedidoMestre.Api.Middleware
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = HttpStatusCode.InternalServerError;
            var message = "Ocorreu um erro interno no servidor";

            switch (exception)
            {
                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    message = exception.Message;
                    break;

                case ArgumentNullException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;

                case ArgumentException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;

                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = exception.Message;
                    break;

                default:
                    message = exception.Message;
                    break;
            }

            var response = new ResponseModel<object>(message);
            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}

