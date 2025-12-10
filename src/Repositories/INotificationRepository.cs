using BackOffice.Entities;

namespace BackOffice.Repositories;

public interface INotificationRepository
{
	public Task<Notification> CreateAsync(Notification notification);
	public Task DeleteAsync(Notification notification);
	public Task<List<Notification>> GetBatchAsync(int limit = 10);
}