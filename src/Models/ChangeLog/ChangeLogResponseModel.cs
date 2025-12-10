namespace BackOffice.Models.ChangeLog;

public class ChangeLogResponseModel
{
	public int Id { get; set; }
	public DateTime Created { get; set; }
	public string ChangeType { get; set; }
	public string RuleName { get; set; }
	public string NewValue { get; set; }
	public string OldValue { get; set; }
}