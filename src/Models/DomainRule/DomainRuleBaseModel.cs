using System.Text.Json.Nodes;

namespace BackOffice.Models.DomainRule;

public class DomainRuleBaseModel
{
	public required JsonObject Data { get; set; }
}