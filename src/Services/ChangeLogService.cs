using BackOffice.Entities;
using BackOffice.Repositories;

namespace BackOffice.Services;

public class ChangeLogService : IChangeLogService
{
	private readonly IChangeLogRepository _changeLogRepository;

	public ChangeLogService(
		IChangeLogRepository changeLogRepository)
	{
		_changeLogRepository = changeLogRepository;
	}
	public async Task<List<ChangeLog>> GetAsync(ChangeLogQueryModel changeLogQueryModel)
	{
		return await _changeLogRepository.GetByQueryAsync(changeLogQueryModel);
	}
}