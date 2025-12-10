using BackOffice.Entities;

namespace BackOffice.Common
{
	public class CriticalNotificationClient(HttpClient httpClient)
	{
		public async Task Notify(ChangeLog log)
		{
			await httpClient.PostAsJsonAsync("/api/notification", log);
		}
	}
}
