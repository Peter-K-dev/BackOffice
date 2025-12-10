using System.ComponentModel.DataAnnotations;

namespace BackOffice.Common
{
	public class ProblemDetailException : Exception
	{
		public int Status { get; }
		public string Title { get; }
		public new string Message { get; }
		public string Type { get; }

		public List<ValidationResult>? Errors { get; }

		public ProblemDetailException(int status, string title, List<ValidationResult> errors, string type) :
			this(status, title, "", type)
		{
			Errors = errors;
		}

		public ProblemDetailException(int status, string title, string message, string type) : base(message)
		{
			Status = status;
			Title = title;
			Message = message;
			Type = type;
		}
	}
}
