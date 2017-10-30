using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;

namespace NBitcoin.Utilities
{
	/// <summary>Describes the test results logger class.</summary>
	public class TestResultsLogger : IDisposable
	{
		private readonly StreamWriter _logWriter;

		private TestResultsLogger(string fileName)
		{
			_logWriter = File.CreateText(fileName);
		}

		public void LogLine(string message, [CallerMemberName]string callerName = "")
		{
			_logWriter.WriteLine(
				String.Format(
					"{0}\t{1}\t{2}",
					DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture),
					callerName,
					message));
		}

		public static TestResultsLogger Create(string logName)
		{
			string logFileName = String.Format("{0}_{1}.txt", logName, DateTime.UtcNow.ToString("yyyyMMdd-HHmmss"));
			return new TestResultsLogger(logFileName);
		}

		public void Dispose()
		{
			_logWriter.Flush();
			_logWriter.Dispose();
		}
	}
}