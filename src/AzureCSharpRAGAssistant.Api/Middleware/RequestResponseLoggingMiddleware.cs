namespace AzureCSharpRAGAssistant.Api.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var reqTime = DateTime.UtcNow;

            _logger.LogInformation(
                "Incoming Request {Method} {Path}",
                context.Request.Method,
                context.Request.Path
            );

            await _next(context);

            var duration = DateTime.UtcNow - reqTime;

            _logger.LogInformation(
                "Requet Completed {Method} {Path} with Status: {StatusCode} in {duration} ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                duration.TotalMicroseconds);
        }
    }
}