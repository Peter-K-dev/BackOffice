using BackOffice.Middleware;

namespace BackOffice.Common
{
	public class CorrelationIdDelegatingHandler : DelegatingHandler
	{
		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{

			if (!request.Headers.Contains(CorrelationIdMiddleware.CorrelationIdHeaderName))
			{
				request.Headers.Add(CorrelationIdMiddleware.CorrelationIdHeaderName, CorrelationIdContext.Get());
			}

			return await base.SendAsync(request, cancellationToken);
		}
	}
}
