using BackOffice.Entities;
using System.Text.Json;
using System.Text.Json.Nodes;
using BackOffice.Repositories;
using BackOffice.Common;
using BackOffice.Database;
using BackOffice.Exceptions;
using Namotion.Reflection;

namespace BackOffice.Services;

public class DomainRuleService : IDomainRuleService
{
	private readonly IDomainRuleRepository _domainRuleRepository;
	private readonly MemoryDbContext _memoryDbContext;
	private readonly ILogger _logger;
	private readonly INotificationRepository _notificationRepository;

	public DomainRuleService(
		IDomainRuleRepository domainRuleRepository,
		MemoryDbContext memoryDbContext,
		ILogger<DomainRuleService> logger, INotificationRepository notificationRepository)
	{
		_domainRuleRepository = domainRuleRepository;
		_memoryDbContext = memoryDbContext;
		_logger = logger;
		_notificationRepository = notificationRepository;
	}
	public async Task<DomainRule?> GetAsync(int id)
	{
		_logger.LogInformation($"DomainRule get id[{id}]");
		return await _domainRuleRepository.GetByIdAsync(id);
	}

	

	public async Task<int> CreateAsync(DomainRule domainRule)
	{
		_logger.LogInformation($"DomainRule id[{domainRule.Id}] Create Start");

		var ruleActual = await _domainRuleRepository.GetByNameAsync(domainRule.Name);
		if (ruleActual != null)
			throw new DomainRuleExistsException();

		
		domainRule.Created = DateTime.UtcNow;

		var createdRule = await _domainRuleRepository.CreateAsync(domainRule);

		var changeLog = new ChangeLog()
		{
			RuleName = domainRule.Name,
			Created = DateTime.UtcNow,
			ChangeType = "create",
			NewValue = domainRule.Data,
			OldValue = ""
		};

		_memoryDbContext.Add(changeLog);

		await _memoryDbContext.SaveChangesAsync();

		_logger.LogInformation($"DomainRule id[{createdRule.Id}] Created");

		if (CriticalDomainRuleDetection.IsCritical(domainRule.Data))
		{
			var notification = new Notification
			{
				CorrelationId = CorrelationIdContext.Get(),
				CreatedTime = DateTime.UtcNow,
				Data = JsonSerializer.Serialize(changeLog)
			};

			await _notificationRepository.CreateAsync(notification);
		}



		return createdRule.Id;
	}


	public async Task UpdateAsync(DomainRule domainRule)
	{
		_logger.LogInformation($"DomainRule id[{domainRule.Id}] Updating Start");
		var currentRule = await _domainRuleRepository.GetByIdAsync(domainRule.Id);

		if (currentRule == null)
		{
			_logger.LogWarning($"DomainRule id[{domainRule.Id}] Not Found");
			throw new DomainRuleNotFoundException();
		}

		if (!currentRule.RowVersion.SequenceEqual(domainRule.RowVersion))
		{
			_logger.LogWarning($"DomainRule id[{domainRule.Id}] was modified ");
			throw new DomainRuleModifiedException();
		}

		var oldData = currentRule.Data;
		currentRule.Data = domainRule.Data;
		currentRule.Updated = DateTime.UtcNow;
		currentRule.RowVersion = BitConverter.GetBytes(DateTime.UtcNow.Ticks); ;

		await _domainRuleRepository.UpdateAsync(currentRule);

		_logger.LogInformation($"DomainRule id[{currentRule.Id}] Updated");

		var changeLog = new ChangeLog()
		{
			RuleName = currentRule.Name,
			Created = DateTime.UtcNow,
			ChangeType = "update",
			NewValue = currentRule.Data,
			OldValue = oldData
		};

		_memoryDbContext.Add(changeLog);

		await _memoryDbContext.SaveChangesAsync();

		if (CriticalDomainRuleDetection.IsCritical(domainRule.Data))
		{
			var notification = new Notification
			{
				CorrelationId = CorrelationIdContext.Get(),
				CreatedTime = DateTime.UtcNow,
				Data = JsonSerializer.Serialize(changeLog)
			};

			await _notificationRepository.CreateAsync(notification);
		}
	}

	public async Task DeleteAsync(int id)
	{
		_logger.LogInformation($"DomainRule id[{id}] Delete Start");

		var currentRule = await _domainRuleRepository.GetByIdAsync(id);

		if (currentRule == null)
		{
			_logger.LogWarning($"DomainRule id[{id}] Not Found");
			throw new DomainRuleNotFoundException();
		}


		await _domainRuleRepository.DeleteAsync(currentRule);

		

		_logger.LogInformation($"DomainRule id[{id}] Deleted");

		var changeLog = new ChangeLog()
		{
			RuleName = currentRule.Name,
			Created = DateTime.UtcNow,
			ChangeType = "delete",
			NewValue = "",
			OldValue = currentRule.Data
		};

		_memoryDbContext.Add(changeLog);

		await _memoryDbContext.SaveChangesAsync();

	}

	public async Task<List<DomainRule>> GetAsync(DomainRuleQueryModel domainRuleQueryModel)
	{
		return await _domainRuleRepository.GetByQueryAsync(domainRuleQueryModel);
	}
}