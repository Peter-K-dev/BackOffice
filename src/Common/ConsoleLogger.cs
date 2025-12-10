namespace BackOffice.Common
{
	public sealed class ConsoleLogger(string name, Func<ConsoleLoggerConfiguration> currentConfig) : ILogger
	{
		public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

		public bool IsEnabled(LogLevel logLevel) => (int)logLevel >= currentConfig().Loglevel;

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
		{
			if(!IsEnabled(logLevel))
				return;

			var correlationId = CorrelationIdContext.Get();


			Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss} {logLevel.ToString().ToUpper()[..3]} {correlationId}] {exception?.Message}");
			Console.WriteLine($"{formatter(state, exception)} - {name}");
			if(currentConfig().StackTrace)
				Console.WriteLine(exception?.StackTrace);
			Console.WriteLine();
		}
	}

	public class ConsoleLoggerConfiguration
	{
		public int Loglevel { get; set; } = 0;

		public bool StackTrace { get; set; }
	}
}
