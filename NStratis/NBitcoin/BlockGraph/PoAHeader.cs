using NBitcoin.Crypto;

namespace NBitcoin
{
	/// <summary>Describes the proof of authority header class.</summary>
	public class PoAHeader : IBitcoinSerializable, IHashable
	{
		private uint256 _authorityHash;

		/// <summary>Initializes a new instance of the <see cref="PoAHeader" /> class.</summary>
		public PoAHeader()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="PoAHeader" /> class.</summary>
		/// <param name="authorityHash">Authority hash.</param>
		public PoAHeader(uint256 authorityHash)
		{
			AuthorityHash = authorityHash;
		}

		/// <summary>Gets or sets the </summary>
		public uint256 AuthorityHash
		{
			get { return _authorityHash; }
			set { _authorityHash = value; }
		}

		/// <summary>Gets a value indicating the <see cref="PoAHeader"/> is empty.</summary>
		public bool IsEmpty
		{
			get { return AuthorityHash == null || AuthorityHash.Equals(uint256.Zero); }
		}

		public uint256 GetHash()
		{
			return Hashes.Hash256(this.ToBytes());
		}

		/// <inheritdoc />
		public void ReadWrite(BitcoinStream stream)
		{
			stream.ReadWrite(ref _authorityHash);
		}
	}
}