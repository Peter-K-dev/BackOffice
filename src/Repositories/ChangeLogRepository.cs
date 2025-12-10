using BackOffice.Database;
using Microsoft.EntityFrameworkCore;
using BackOffice.Entities;

namespace BackOffice.Repositories;

public class ChangeLogRepository : IChangeLogRepository
{
	private readonly MemoryDbContext _memoryDbContext;
	public ChangeLogRepository(MemoryDbContext memoryDbContext)
	{
		_memoryDbContext = memoryDbContext;
	}
	public async Task<List<ChangeLog>> GetByQueryAsync(ChangeLogQueryModel changeLogQueryModel)
	{
		var query = _memoryDbContext.ChangeLogs.AsNoTracking();

		if (changeLogQueryModel.OrderBy.Equals("type", StringComparison.InvariantCultureIgnoreCase))
		{
			query = query.OrderBy(c => c.ChangeType);
		}
		else
		{
			query = query.OrderBy(c => c.Created);
		}

		return await query.Skip(changeLogQueryModel.Offset * changeLogQueryModel.Limit).Take(changeLogQueryModel.Limit - 1).ToListAsync();
	}
}