using NBitcoin.Crypto;

namespace NBitcoin
{
	/// <summary>Default <see cref="IBlockContent"/> implementation.</summary>
	public class GraphContent : IBlockContent
	{
		private GraphContentHeaders _headers = new GraphContentHeaders();

		private byte[] _graph = new byte[0];

		private uint _version;

		/// <summary>Gets or sets the graph contant headers.</summary>
		public GraphContentHeaders Headers
		{
			get { return _headers; }
			set { _headers = value; }
		}

		/// <summary>Gets or sets the graph stream.</summary>
		public byte[] Graph
		{
			get { return _graph; }
			set { _graph = value; }
		}

		/// <summary>Gets or sets the content version.</summary>
		public uint Version
		{
			get { return _version; }
			set { _version = value; }
		}

		public uint256 GetHash()
		{
			return Hashes.Hash256(this.ToBytes());
		}

		/// <inheritdoc />
		public void ReadWrite(BitcoinStream stream)
		{
			stream.ReadWrite(ref _headers);
			stream.ReadWrite(ref _graph);
		}
	}
}