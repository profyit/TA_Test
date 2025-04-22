using System.Net;
using System.Text.Json;
namespace TaskManagement.Middleware
{
    // Middleware/GlobalExceptionMiddleware.cs
    

    namespace TaskManagerApi.Middleware
    {
        public class GlobalExceptionMiddleware
        {
            private readonly RequestDelegate _next;
            private readonly ILogger<GlobalExceptionMiddleware> _logger;
            private readonly IHostEnvironment _env;
       

            public GlobalExceptionMiddleware(RequestDelegate next,
                                             ILogger<GlobalExceptionMiddleware> logger,
                                             IHostEnvironment env)
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
                    _logger.LogError(ex, "An unhandled exception occurred. Path: {Path}", context.Request.Path);
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; 

                   
                    var response = new
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = "An internal server error occurred. Please try again later.",

                        Details = _env.IsDevelopment() ? ex.ToString() : null
                    };

                    var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
                    });

                    await context.Response.WriteAsync(jsonResponse);
                }
            }
        }
    }
}
