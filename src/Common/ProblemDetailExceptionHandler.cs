using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using BackOffice.Middleware;

namespace BackOffice.Common
{
	public class ProblemDetailExceptionHandler : IExceptionHandler
	{
		private readonly IProblemDetailsService _problemDetailsService;
		private readonly ILogger _logger;

		public ProblemDetailExceptionHandler(IProblemDetailsService problemDetailsService,
			ILogger<ProblemDetailExceptionHandler> logger
		)
		{
			_problemDetailsService = problemDetailsService;
			_logger = logger;
		}
		public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
		{
			ProblemDetails? problemDetails;
			if (exception is not ProblemDetailException problemException)
			{
				problemDetails = new ProblemDetails
				{
					Status = (int)HttpStatusCode.InternalServerError,
					Type = exception.GetType().Name,
					Title = "An unhandled error occurred",
					Detail = exception.Message
				};

				httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

				_logger.LogError(exception, "An unhandled error occurred");
			}
			else
			{
				problemDetails = new ProblemDetails
				{
					Status = problemException.Status,
					Title = problemException.Title,
					Detail = problemException.Message,
					Type = problemException.Type
				};

				if (problemException.Errors != null)
					problemDetails.Extensions.TryAdd("error", problemException.Errors);

				httpContext.Response.StatusCode = problemException.Status;
				httpContext.Response.Headers.TryAdd(CorrelationIdMiddleware.CorrelationIdHeaderName,
					CorrelationIdContext.Get());
				_logger.LogError(exception, problemException.Message);
			}

			return await _problemDetailsService.TryWriteAsync(
				new ProblemDetailsContext
				{
					HttpContext = httpContext,
					ProblemDetails = problemDetails,
				});

		}
	}
}
