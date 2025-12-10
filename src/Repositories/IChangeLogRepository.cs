using BackOffice.Entities;


namespace BackOffice.Repositories;

public interface IChangeLogRepository
{
	public Task<List<ChangeLog>> GetByQueryAsync(ChangeLogQueryModel changeLogQueryModel);
}