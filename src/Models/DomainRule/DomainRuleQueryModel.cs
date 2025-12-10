using System.ComponentModel.DataAnnotations;

public class DomainRuleQueryModel
{
	[Range(0, Int32.MaxValue)]
	public int Offset { get; set; } = 0;
	[Range(1, Int32.MaxValue)]
	public int Limit { get; set; } = 100;
}