using BackOffice.Entities;
using BackOffice.Mapping;
using BackOffice.Models.DomainRule;
using BackOffice.Repositories;
using BackOffice.Services;
using Microsoft.AspNetCore.Mvc;



namespace BackOffice.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ChangeLogsController : ControllerBase

	{
		private readonly IChangeLogService _changeLogService;
		private readonly INotificationRepository _notificationRepository;



		public ChangeLogsController(
			IChangeLogService changeLogService, INotificationRepository notificationRepository)
		{
			_changeLogService = changeLogService;
			_notificationRepository = notificationRepository;
		}

		[HttpGet]
		[ProducesResponseType<IEnumerable<ChangeLog>>(statusCode: StatusCodes.Status200OK)]
		public async Task<ActionResult<IEnumerable<ChangeLog>>> GetByQuery(CancellationToken cancellationToken,
			[FromQuery] ChangeLogQueryModel changeLogQueryModel)
		{
			var items = await _changeLogService.GetAsync(changeLogQueryModel);

			var result = items.Select(itm => itm.MapToChangeLogResponseModel()).ToList();

			return Ok(result);
		}

		[HttpGet("notifications")]
		public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications(CancellationToken cancellationToken)
		{
			return Ok(await _notificationRepository.GetBatchAsync());
		}
	}
}
