using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackOffice.Entities
{
	public class ChangeLog
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		[Required]
		public string RuleName { get; set; }
		[Required]
		public string ChangeType { get; set; }
		[Required]
		public string OldValue { get; set; }
		[Required]
		public string NewValue { get; set; }

		public DateTime Created { get; set; } = DateTime.UtcNow;
	}
}
