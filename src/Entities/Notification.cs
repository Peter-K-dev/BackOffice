using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackOffice.Entities;

public class Notification
{
	[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }

	public string CorrelationId { get; set; }

	[Required]
	public string Data { get; set; }


	public DateTime CreatedTime { get; set; }

}
