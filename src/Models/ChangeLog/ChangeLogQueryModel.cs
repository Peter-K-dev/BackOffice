using System.ComponentModel.DataAnnotations;

public class ChangeLogQueryModel
{

	public string OrderBy { get; set; } = "created";

	[Range(0, Int32.MaxValue)]
	public int Offset { get; set; } = 0;
	[Range(1, Int32.MaxValue)]
	public int Limit { get; set; } = 100;
}