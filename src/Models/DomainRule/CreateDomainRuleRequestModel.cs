using System.ComponentModel.DataAnnotations;

namespace BackOffice.Models.DomainRule
{
	public class CreateDomainRuleRequestModel : DomainRuleBaseModel
	{
		[Required(AllowEmptyStrings = false)]
		public required string Name { get; set; }
	}

}
