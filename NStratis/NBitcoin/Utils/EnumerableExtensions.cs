using System.Collections.Generic;

namespace NBitcoin
{
	/// <summary>Provides with useful extensions for <see cref="IEnumerable"/></summary>
	public static class EnumerableExtensions
	{
		/// <summary>Prepends items to the collection.</summary>
		/// <typeparam name="T">Type of elements.</typeparam>
		/// <param name="collection">Collection of elements.</param>
		/// <param name="items">Prepended items array.</param>
		/// <returns>Prepended collection.</returns>
		public static IEnumerable<T> Prepend<T>(this IEnumerable<T> collection, params T[] items)
		{
			foreach (T item in items)
			{
				yield return item;
			}

			foreach (T item in collection)
			{
				yield return item;
			}
		}

		/// <summary>Appends items to the collection.</summary>
		/// <typeparam name="T">Type of elements.</typeparam>
		/// <param name="collection">Collection of elements.</param>
		/// <param name="items">Appended items array.</param>
		/// <returns>Appended collection.</returns>
		public static IEnumerable<T> Append<T>(this IEnumerable<T> collection, params T[] items)
		{
			foreach (T item in collection)
			{
				yield return item;
			}

			foreach (T item in items)
			{
				yield return item;
			}
		}
	}
}