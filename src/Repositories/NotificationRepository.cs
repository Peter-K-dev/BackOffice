using BackOffice.Database;
using Microsoft.EntityFrameworkCore;
using BackOffice.Entities;


namespace BackOffice.Repositories;

public class NotificationRepository : INotificationRepository
{

	private readonly MemoryDbContext _memoryDbContext;

	public NotificationRepository(MemoryDbContext memoryDbContext)
	{
		_memoryDbContext = memoryDbContext;
	}

	public async Task<Notification> CreateAsync(Notification notification)
	{
		_memoryDbContext.Notifications.Add(notification);

		await _memoryDbContext.SaveChangesAsync();

		return notification;
	}

	public async Task DeleteAsync(Notification notification)
	{

		_memoryDbContext.Notifications.Remove(notification);

		await _memoryDbContext.SaveChangesAsync();

	}

	public async Task<List<Notification>> GetBatchAsync(int limit = 10)
	{
		return await _memoryDbContext.Notifications.AsNoTracking().OrderBy(n => n.Id).Take(limit)
			.ToListAsync();
	}
}