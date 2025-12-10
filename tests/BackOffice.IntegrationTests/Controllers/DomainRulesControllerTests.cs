using BackOffice.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Json;

namespace BackOffice.IntegrationTests.Controllers
{
	public class DomainRulesControllerTests
	{
		private WebApplicationFactory<Program> webApplicationFactory;
		private HttpClient _client;

		[OneTimeSetUp]
		public void Setup()
		{
			webApplicationFactory = new WebApplicationFactory<Program>();
			{
				_client = webApplicationFactory.WithWebHostBuilder(static builder =>
					{

						builder.ConfigureServices(services =>
						{

							services.AddDbContext<MemoryDbContext>(options =>
								options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
						});

					})
					.CreateClient();
			}
		}

		[OneTimeTearDown]
		public void Cleanup()
		{
			webApplicationFactory?.Dispose();
			_client?.Dispose();
		}

		[Test]
		public async Task FullRuleWorkflow_ShouldSucceed()
		{
			var createPayload = new
			{
				name = "creditLimits-" + Guid.NewGuid(),
				data = new { minCredit = 1500 }
			};

			var client = webApplicationFactory.CreateClient();

			var createRequest = await client.PostAsJsonAsync("/api/domainrules", createPayload);
			createRequest.EnsureSuccessStatusCode();

			using var scope = webApplicationFactory.Services.CreateScope();
			var dbContext = scope.ServiceProvider.GetRequiredService<MemoryDbContext>();

			var newRule = await dbContext.DomainRules.FirstOrDefaultAsync(r => r.Name == createPayload.name);

			Assert.That(newRule, Is.Not.Null);

			var newLog = await dbContext.ChangeLogs.FirstOrDefaultAsync(c => c.RuleName == createPayload.name);

			Assert.That(newLog, Is.Not.Null);

			var updatePayload = new
			{
				id = newRule.Id,
				name = newRule.Name,
				data = new { minCredit = 1400 },
				rowVersion = newRule.RowVersion
			};

			var updateRequest = await client.PutAsJsonAsync("/api/domainrules", updatePayload);
			updateRequest.EnsureSuccessStatusCode();

			var updateLog = await dbContext.ChangeLogs.FirstOrDefaultAsync(c => c.RuleName == newRule.Name && c.ChangeType == "update");

			Assert.That(updateLog, Is.Not.Null);

			var deleteRequest = await client.DeleteAsync($"/api/domainrules/{newRule.Id}");
			deleteRequest.EnsureSuccessStatusCode();

			var deleteLog = await dbContext.ChangeLogs.FirstOrDefaultAsync(c => c.RuleName == newRule.Name && c.ChangeType == "delete");

			Assert.That(deleteLog, Is.Not.Null);
		}
	}
}
