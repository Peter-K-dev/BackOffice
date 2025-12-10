
namespace BackOffice.Middleware
{
	public class LoggingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger _logger;

		public LoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
		{
			_next = next;
			_logger = loggerFactory.CreateLogger<LoggingMiddleware>();
		}

		public async Task Invoke(HttpContext context)
		{
			_logger.LogInformation("Request start: " + context.Request.Method + " " + context.Request.Path);
			await _next.Invoke(context);
			_logger.LogInformation("Request end.");
		}
	}
}
