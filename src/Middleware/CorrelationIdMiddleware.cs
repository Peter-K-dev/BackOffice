using BackOffice.Common;

namespace BackOffice.Middleware
{
	public class CorrelationIdMiddleware
	{
		private readonly RequestDelegate _next;
		public const string CorrelationIdHeaderName = "X-CorrelationID";

		public CorrelationIdMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			//using (LogContext.PushProperty("CorrelationId", correlationId))

			var correlationId = GetCorrelationId(context);

			CorrelationIdContext.Set(correlationId);

			context.Response?.Headers?.TryAdd(CorrelationIdHeaderName, correlationId);
			await _next(context);

		}

		private string GetCorrelationId(HttpContext context)
		{
			if (context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var correlationId))
				return correlationId.FirstOrDefault() ?? Guid.NewGuid().ToString();

			return Guid.NewGuid().ToString();
		}
	}
}
