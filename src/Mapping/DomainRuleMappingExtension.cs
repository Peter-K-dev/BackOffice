using BackOffice.Entities;
using BackOffice.Models.DomainRule;
using System.Data;
using System.Text.Json.Nodes;
using BackOffice.Exceptions;

namespace BackOffice.Mapping
{
	public static class DomainRuleMappingExtension
	{
		public static DomainRuleResponseModel MapToDomainRuleResponseModel(
			this DomainRule source)
		{
			try
			{
				var jObject = JsonNode.Parse(source.Data)!.AsObject();


				return new DomainRuleResponseModel
				{
					Id = source.Id,
					Created = source.Created,
					Name = source.Name,
					Data = jObject,
					RowVersion = source.RowVersion
				};
			}
			catch
			{
				throw new DomainRuleInvalidJsonException();
			}
			
		}

		public static DomainRule MapToDomainRule(
			this CreateDomainRuleRequestModel source)
		{
			return new DomainRule()
			{
				Name = source.Name,
				Data = source.Data.ToJsonString(),
			};
		}

		public static DomainRule MapToDomainRule(
			this UpdateDomainRuleRequestModel source)
		{
			return new DomainRule()
			{
				Id = source.Id,
				Data = source.Data.ToJsonString(),
				RowVersion = source.RowVersion
			};
		}
	}

	
}
