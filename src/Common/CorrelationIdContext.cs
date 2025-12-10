namespace BackOffice.Common
{
	public static class CorrelationIdContext
	{
		static readonly AsyncLocal<string> Id = new();


		public static void Set(string value)
		{
			Id.Value = value;
		}

		public static string Get()
		{
			return Id.Value ?? "";
		}
	}
}
