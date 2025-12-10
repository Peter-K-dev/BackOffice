using BackOffice.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BackOffice.Common
{
	public class MemoryDbHealthCheck(MemoryDbContext memoryDbContext) : IHealthCheck
	{
		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
		{
			try
			{
				await memoryDbContext.DomainRules.AnyAsync();

				return HealthCheckResult.Healthy();
			}
			catch
			{
				return HealthCheckResult.Unhealthy();
			}


		}
	}
}
