using System;
using System.Diagnostics;

namespace GraphSerialization.Tests
{
	/// <summary>Describes useful stopwatch extension methods.</summary>
	public static class StopwatchExtensions
	{
		/// <summary>Gets the time needed to perform an action in miliseconds.</summary>
		/// <param name="stopWatch">Source stopwatch.</param>
		/// <param name="action">Action delegate.</param>
		/// <param name="iterations">Number of iterations. Default: 1.</param>
		/// <returns>Time needed to perform an action in miliseconds</returns>
		public static long GetMiliseconds(this Stopwatch stopWatch, Action action, int iterations = 1)
		{
			stopWatch.Reset();
			stopWatch.Start();
			for (int i = 0; i < iterations; i++)
			{
				action();
			}

			stopWatch.Stop();

			return stopWatch.ElapsedMilliseconds;
		}
	}
}