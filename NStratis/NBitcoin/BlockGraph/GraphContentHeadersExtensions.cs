using System;
using System.Collections.Generic;
using System.Linq;
using static NBitcoin.GraphContentHeaders;

namespace NBitcoin
{
	/// <summary>Provides with useful <see cref="GraphContentHeaders"/> extension methods.</summary>
	public static class GraphContentHeadersExtensions
	{
		/// <summary>Checks whether the collection of <see cref="GraphContentHeader"/> contains the element with the specified header name.</summary>
		/// <param name="collection">Source <see cref="GraphContentHeader"/> collection.</param>
		/// <param name="name">Header name.</param>
		/// <returns>Value indicating whether the collection of <see cref="GraphContentHeader"/> contains the element with the specified header name.</returns>
		public static bool ContainsName(this List<GraphContentHeader> collection, string name)
		{
			ThrowNullOrEmptyArgument(name, nameof(name));

			return collection.Any(p => name.Equals(p.Name, StringComparison.OrdinalIgnoreCase));
		}

		/// <summary>Removes the header from the collection.</summary>
		/// <param name="collection">Source <see cref="GraphContentHeader"/> collection.</param>
		/// <param name="name">Header name.</param>
		/// <returns>Value indicating whether the removal was successful. If a header with the given name was not present in the collection, returns false.</returns>
		public static bool RemoveByName(this List<GraphContentHeader> collection, string name)
		{
			ThrowNullOrEmptyArgument(name, nameof(name));

			bool success = false;
			if (collection.ContainsName(name))
			{
				try
				{
					collection.Remove(collection.Find(p => name.Equals(p.Name, StringComparison.OrdinalIgnoreCase)));
					success = true;
				}
				catch
				{
					success = false;
				}
			}

			return success;
		}

		/// <summary>Adds or updates the header in collection.</summary>
		/// <param name="collection">Source <see cref="GraphContentHeader"/> collection.</param>
		/// <param name="name">Header name.</param>
		/// <param name="values">Array of header values.</param>
		/// <returns>Added or updated <see cref="GraphContentHeader"/>.</returns>
		public static GraphContentHeader AddOrUpdate(this List<GraphContentHeader> collection, string name, params string[] values)
		{
			ThrowNullOrEmptyArgument(name, nameof(name));
			GraphContentHeader returnValue = null;

			var header = collection.FirstOrDefault(p => name.Equals(p.Name, StringComparison.OrdinalIgnoreCase));
			if (header != null)
			{
				header.Value = values.ToList();
			}
			else
			{
				header = new GraphContentHeader() { Name = name, Value = values.ToList() };
				collection.Add(header);
			}

			returnValue = header;
			return returnValue;
		}

		/// <summary>Throws <see cref="ArgumentNullException"/> when the argument is null or empty.</summary>
		/// <param name="argument">Given argument.</param>
		/// <param name="argumentName">Argument name.</param>
		private static void ThrowNullOrEmptyArgument(string argument, string argumentName)
		{
			if (String.IsNullOrEmpty(argument))
			{
				throw new ArgumentNullException(argumentName);
			}
		}
	}
}