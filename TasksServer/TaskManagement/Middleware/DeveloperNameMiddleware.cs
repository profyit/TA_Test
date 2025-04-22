

namespace TaskManagement.Middleware
{
    
    public class DeveloperNameMiddleware
    {


        private readonly RequestDelegate _next;
        private const string DeveloperHeaderName = "Name-Developer";

        public DeveloperNameMiddleware(RequestDelegate next)
        {
            _next = next;
        }

    
        public async Task InvokeAsync(HttpContext context, ICustomLogService logService)
        {
  
            string developerName = "Unknown";
            if (context.Request.Headers.TryGetValue(DeveloperHeaderName, out var headerValues))
            {
                developerName = headerValues.FirstOrDefault() ?? "Unknown";
            }


            await _next(context);

            logService.LogApiCall(new LogWebEntity
            {
                DeveloperName = developerName,
                QueryString = context.Request.QueryString.ToString(),
                Path = context.Request.Path, 
                StatusCode = context.Response.StatusCode,
                Method = context.Request.Method,
                
            });
        }

    }

}
