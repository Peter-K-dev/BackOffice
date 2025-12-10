using System.Collections.Concurrent;

namespace BackOffice.Common
{
	public class ConsoleLoggerProvider : ILoggerProvider
	{
		private readonly ConcurrentDictionary<string, ConsoleLogger> _loggers =
			new(StringComparer.OrdinalIgnoreCase);

		public void Dispose()
		{
			_loggers.Clear();
		}

		public ILogger CreateLogger(string categoryName) =>
			_loggers.GetOrAdd(categoryName, name => new ConsoleLogger(name, () =>  new ConsoleLoggerConfiguration
			{
				Loglevel = 0,
				StackTrace = false
			}));

	}
}
