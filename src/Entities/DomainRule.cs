using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Entities
{
	[Index(nameof(Name), IsUnique = true)]
	public class DomainRule
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public string Name { get; set; }

		[Required]
		public string Data { get; set; }

		[Timestamp] public byte[] RowVersion { get; set; } = new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 };

		public DateTime Created { get; set; }
		public DateTime? Updated { get; set; }
	}
}
