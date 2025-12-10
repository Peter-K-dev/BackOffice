using System.Collections.Concurrent;
using BackOffice.Common;
using BackOffice.Database;
using BackOffice.Entities;
using BackOffice.Middleware;
using BackOffice.Repositories;
using BackOffice.Services;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Metrics;

namespace BackOffice.DependencyInjections;

public static class BackOfficeExtensions
{
	public static WebApplicationBuilder AddBackOffice(this WebApplicationBuilder builder)
	{
		builder.Services.AddDbContext<MemoryDbContext>(
			options => options
				.UseInMemoryDatabase("DefaultConnection"));

		builder.Services.AddTransient<CorrelationIdDelegatingHandler>();

		builder.Services.AddHttpClient<CriticalNotificationClient>(httpClient =>
				{
					httpClient.BaseAddress = new Uri(builder.Configuration["notificationServiceHost"] ?? throw new InvalidOperationException());

				}
			).AddHttpMessageHandler<CorrelationIdDelegatingHandler>()
			.AddStandardResilienceHandler();


		builder.Services.AddScoped<IDomainRuleService, DomainRuleService>();
		builder.Services.AddScoped<IChangeLogService, ChangeLogService>();

		builder.Services.AddHostedService<NotificationService>();


		builder.Services.AddScoped<IDomainRuleRepository, DomainRuleRepository>();
		builder.Services.AddScoped<IChangeLogRepository, ChangeLogRepository>();
		builder.Services.AddScoped<INotificationRepository, NotificationRepository>();


		builder.Services.AddProblemDetails(options =>
			options.CustomizeProblemDetails = context =>
			{
				context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
			});


		builder.Services.AddLogging(loggingBuilder =>
		{
			loggingBuilder.ClearProviders();

			loggingBuilder.AddProvider(new ConsoleLoggerProvider());

		});


		builder.Services.AddExceptionHandler<ProblemDetailExceptionHandler>();
		builder.Services.AddHealthChecks()
			.AddCheck<MemoryDbHealthCheck>("database", HealthStatus.Unhealthy);

		builder.Services.AddOpenTelemetry()
			.WithMetrics(builder =>
			{
				builder.AddPrometheusExporter();

				builder.AddMeter("Microsoft.AspNetCore.Hosting",
					"Microsoft.AspNetCore.Server.Kestrel",
					"BackOffice.Api");
				builder.AddView("http.server.request.duration",
					new ExplicitBucketHistogramConfiguration
					{
						Boundaries = new double[] { 0, 0.005, 0.01, 0.025, 0.05,
							0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10 }
					});
			});

		builder.Services.AddMetrics();

		return builder;
	}

	public static WebApplication AddBackOffice(this WebApplication app)
	{
		var databaseScope = app.Services.CreateScope();
		var context = databaseScope
			.ServiceProvider
			.GetRequiredService<MemoryDbContext>();

		app.UseMiddleware<CorrelationIdMiddleware>();
		app.UseMiddleware<LoggingMiddleware>();

		app.AddSimulate();

		app.MapHealthChecks("health", new HealthCheckOptions()
		{
			ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
		});

		app.UseExceptionHandler();

		app.MapPrometheusScrapingEndpoint();

		return app;
	}

	public static WebApplication AddSimulate(this WebApplication app)
	{
		app.MapPost("api/notification", ([FromBody] ChangeLog log, HttpContext context) =>
		{

			context.Response.StatusCode = StatusCodes.Status200OK;
			return Task.FromResult(Results.Ok(new { status = "success" }));
		});
		return app;
	}
}