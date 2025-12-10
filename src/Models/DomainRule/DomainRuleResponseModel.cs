using System.ComponentModel.DataAnnotations;

namespace BackOffice.Models.DomainRule;

public class DomainRuleResponseModel : DomainRuleBaseModel
{
	public required int Id { get; set; }

	[Required(AllowEmptyStrings = false)]
	public required string Name { get; set; }
	public required DateTime Created { get; set; }
	public byte[] RowVersion { get; set; }
}