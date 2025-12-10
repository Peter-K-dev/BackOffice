using BackOffice.Common;
using BackOffice.Entities;
using BackOffice.Repositories;
using System.Diagnostics.Metrics;
using System.Text.Json;

namespace BackOffice.Services;

public class NotificationService : BackgroundService
{
	private readonly ILogger<NotificationService> _logger;
	private readonly CriticalNotificationClient _criticalNotificationClient;
	private readonly IServiceScopeFactory _serviceScopeFactory;
	private readonly IMeterFactory _meterFactory;
	private readonly Counter<int> _notificationCounter;


	public NotificationService(ILogger<NotificationService> logger,
		CriticalNotificationClient criticalNotificationClient,
		IServiceScopeFactory serviceScopeFactory,
		IMeterFactory meterFactory)
	{
		_logger = logger;
		_criticalNotificationClient = criticalNotificationClient;
		_serviceScopeFactory = serviceScopeFactory;

		_meterFactory = meterFactory;

		var meter = _meterFactory.Create("BackOffice.Api");
		_notificationCounter = meter.CreateCounter<int>("notification");
	}


	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await ProcessAsync();

			await Task.Delay(5_000, stoppingToken);
		}
	}

	public async Task ProcessAsync()
	{
		var scope = _serviceScopeFactory.CreateScope();
		var notificationRepository = scope.ServiceProvider.GetService<INotificationRepository>() ?? throw new ArgumentNullException("scope.ServiceProvider.GetService<INotificationRepository>()");

		var batch = await notificationRepository.GetBatchAsync();

		if (batch.Count != 0)
		{
			try
			{
				foreach (var notification in batch)
				{
					CorrelationIdContext.Set(notification.CorrelationId);

					_logger.LogInformation($"Sending critical notification {notification.CorrelationId}");

					var log = JsonSerializer.Deserialize<ChangeLog>(notification.Data)!;

					await _criticalNotificationClient.Notify(log);

					_notificationCounter.Add(1,
						new KeyValuePair<string, object?>("action", "sended"));

					await notificationRepository.DeleteAsync(notification);

				}

				await ProcessAsync();
			}
			catch
			{
				_notificationCounter.Add(1,
					new KeyValuePair<string, object?>("action", "fail"));
			}
		}
	}
}