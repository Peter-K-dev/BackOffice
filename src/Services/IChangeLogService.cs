using BackOffice.Entities;

namespace BackOffice.Services;

public interface IChangeLogService
{
	Task<List<ChangeLog>> GetAsync(ChangeLogQueryModel changeLogQueryModel);
}