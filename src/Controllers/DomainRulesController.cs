using BackOffice.Common;
using BackOffice.Exceptions;
using BackOffice.Mapping;
using BackOffice.Models.DomainRule;
using BackOffice.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.Metrics;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;


namespace BackOffice.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DomainRulesController : ControllerBase

	{
		private readonly IDomainRuleService _domainRuleService;
		private readonly LinkGenerator _linkGenerator;
		private readonly IMeterFactory _meterFactory;
		private readonly Counter<int> _domainRulesCounter;


		public DomainRulesController(
			IDomainRuleService domainRuleService,
			LinkGenerator linkGenerator, 
			IMeterFactory meterFactory)
		{
			_domainRuleService = domainRuleService;
			_linkGenerator = linkGenerator;
			_meterFactory = meterFactory;

			var meter = _meterFactory.Create("BackOffice.Api");
			_domainRulesCounter = meter.CreateCounter<int>("domainrule");
		}

		[HttpGet]
		[ProducesResponseType<IEnumerable<DomainRuleResponseModel>>(statusCode: StatusCodes.Status200OK)]
		[ProducesResponseType<ProblemDetails>(statusCode: StatusCodes.Status404NotFound)]
		[ProducesResponseType<ProblemDetails>(statusCode: StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<IEnumerable<DomainRuleResponseModel>>> GetByQuery(CancellationToken cancellationToken,
		[FromQuery] DomainRuleQueryModel domainRuleQueryModel)
		{
			var items = await _domainRuleService.GetAsync(domainRuleQueryModel);

			var result = items.Select(itm => itm.MapToDomainRuleResponseModel()).ToList();

			return Ok(result);
		}

		[HttpGet("{id:int}")]
		[ProducesResponseType<DomainRuleResponseModel>(statusCode: StatusCodes.Status200OK)]
		[ProducesResponseType<ProblemDetails>(statusCode: StatusCodes.Status404NotFound)]
		[ProducesResponseType<ProblemDetails>(statusCode: StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<DomainRuleResponseModel>> GetById(CancellationToken cancellationToken,
			int id)
		{

			var rule = await _domainRuleService.GetAsync(id);

			if (rule == null)
				throw new DomainRuleNotFoundException();

			var result = rule.MapToDomainRuleResponseModel();

			return Ok(result);
		}

		[HttpPost]
		[ProducesResponseType(statusCode: StatusCodes.Status201Created)]
		[ProducesResponseType<ProblemDetails>(statusCode: StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Create(CancellationToken cancellationToken,
			[FromBody] CreateDomainRuleRequestModel createDomainRuleRequestModel)
		{
			var validationResults = new List<ValidationResult>();
			if (!Validator.TryValidateObject(createDomainRuleRequestModel,
				    new ValidationContext(createDomainRuleRequestModel), validationResults, true))
			{
				throw new DomainRuleValidationException(validationResults);
			}

			var created = createDomainRuleRequestModel.MapToDomainRule();

			
			var createdId = await _domainRuleService.CreateAsync(created);

			_domainRulesCounter.Add(1,
				new KeyValuePair<string, object?>("action", "added"));

			var newUri = _linkGenerator.GetUriByAction(HttpContext, "GetById", null, new {id = createdId});

			return Created(newUri, null);

		}

		[HttpPut]
		[ProducesResponseType(statusCode: StatusCodes.Status204NoContent)]
		[ProducesResponseType<ProblemDetails>(statusCode: StatusCodes.Status404NotFound)]
		[ProducesResponseType<ProblemDetails>(statusCode: StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Update(CancellationToken cancellationToken, [FromBody] UpdateDomainRuleRequestModel updateDomainRuleRequestModel)
		{
			var validationResults = new List<ValidationResult>();
			if (!Validator.TryValidateObject(updateDomainRuleRequestModel,
					new ValidationContext(updateDomainRuleRequestModel), validationResults, true))
			{
				throw new DomainRuleValidationException(validationResults);
			}

			var updated = updateDomainRuleRequestModel.MapToDomainRule();

			await _domainRuleService.UpdateAsync(updated);

			_domainRulesCounter.Add(1,
				new KeyValuePair<string, object?>("action", "update"));

			return NoContent();
		}

		[HttpDelete("{id:int}")]
		[ProducesResponseType(statusCode: StatusCodes.Status204NoContent)]
		[ProducesResponseType<ProblemDetails>(statusCode: StatusCodes.Status404NotFound)]
		[ProducesResponseType<ProblemDetails>(statusCode: StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id)
		{
			await _domainRuleService.DeleteAsync(id);

			_domainRulesCounter.Add(1,
				new KeyValuePair<string, object?>("action", "deleted"));

			return NoContent();
		}
	}
}
