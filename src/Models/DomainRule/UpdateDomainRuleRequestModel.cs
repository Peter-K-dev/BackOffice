using System.ComponentModel.DataAnnotations;

namespace BackOffice.Models.DomainRule;

public class UpdateDomainRuleRequestModel : DomainRuleBaseModel
{
	[Range(1,int.MaxValue)]
	public required int Id { get; set; }

	[Required]
	public required byte[] RowVersion { get; set; }
}