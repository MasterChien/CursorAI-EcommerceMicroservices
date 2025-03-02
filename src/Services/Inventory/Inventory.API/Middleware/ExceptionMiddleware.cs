using System.Net;
using System.Text.Json;
using Inventory.Core.Exceptions;

namespace Inventory.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";

                var statusCode = HttpStatusCode.InternalServerError;
                var errorMessage = "Đã xảy ra lỗi trong quá trình xử lý yêu cầu của bạn";

                if (ex is InventoryException)
                {
                    statusCode = HttpStatusCode.BadRequest;
                    errorMessage = ex.Message;
                }
                else if (ex is InventoryItemNotFoundException || ex is WarehouseNotFoundException)
                {
                    statusCode = HttpStatusCode.NotFound;
                    errorMessage = ex.Message;
                }
                else if (ex is InsufficientStockException)
                {
                    statusCode = HttpStatusCode.BadRequest;
                    errorMessage = ex.Message;
                }

                context.Response.StatusCode = (int)statusCode;

                var response = new
                {
                    StatusCode = (int)statusCode,
                    Message = errorMessage,
                    Details = _env.IsDevelopment() ? ex.StackTrace : null
                };

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
} 