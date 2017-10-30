using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NBitcoin
{
	/// <summary>Describes the graph content headers.</summary>
	public class GraphContentHeaders : IBitcoinSerializable
	{
		/// <summary>Initializes a new instance of the <see cref="GraphContentHeaders" /> class.</summary>
		public GraphContentHeaders()
		{
			Headers = new List<GraphContentHeader>();
		}

		private List<GraphContentHeader> _headers;

		/// <summary>Gets or sets the </summary>
		public List<GraphContentHeader> Headers
		{
			get { return _headers; }
			private set { _headers = value; }
		}

		public void AddHeader(string name, params string[] values)
		{
			if (values == null || values.Length == 0)
			{
				_headers.RemoveByName(name);
			}
			else
			{
				_headers.AddOrUpdate(name, values);
			}
		}

		/// <inheritdoc />
		public void ReadWrite(BitcoinStream stream)
		{
			stream.ReadWrite(ref _headers);
		}

		/// <summary>Describes the single graph content header.</summary>
		public class GraphContentHeader : IBitcoinSerializable
		{
			private string _name;

			private List<string> _value;

			/// <summary>Gets or sets the header name.</summary>
			public string Name
			{
				get { return _name; }
				set { _name = value; }
			}

			/// <summary>Gets or sets the header value.</summary>
			public List<string> Value
			{
				get { return _value; }
				set { _value = value; }
			}

			/// <inheritdoc />
			public void ReadWrite(BitcoinStream stream)
			{
				var utf8 = Encoding.UTF8;
				var nameBytes = utf8.GetBytes(_name);
				stream.ReadWrite(ref nameBytes);

				var valuesBytesAray = _value.Select(s => utf8.GetBytes(s)).ToArray();
				for (int i = 0; i < valuesBytesAray.Length; i++)
				{
					var obj = valuesBytesAray[i];
					stream.ReadWrite(ref obj);
					valuesBytesAray[i] = obj;
				}
			}
		}
	}
}