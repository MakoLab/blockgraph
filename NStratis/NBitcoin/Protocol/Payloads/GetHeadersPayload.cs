﻿namespace NBitcoin.Protocol
{
	/// <summary>
	/// Ask block headers that happened since BlockLocators
	/// </summary>
	[Payload("getheaders")]
	public class GetHeadersPayload : Payload
	{
		public GetHeadersPayload()
		{
		}

		public GetHeadersPayload(BlockLocator locator)
		{
			BlockLocators = locator;
		}

		private uint version = (uint)ProtocolVersion.PROTOCOL_VERSION;

		public ProtocolVersion Version
		{
			get
			{
				return (ProtocolVersion)version;
			}
			set
			{
				version = (uint)value;
			}
		}

		private BlockLocator blockLocators;

		public BlockLocator BlockLocators
		{
			get
			{
				return blockLocators;
			}
			set
			{
				blockLocators = value;
			}
		}

		private uint256 hashStop = uint256.Zero;

		public uint256 HashStop
		{
			get
			{
				return hashStop;
			}
			set
			{
				hashStop = value;
			}
		}

		public override void ReadWriteCore(BitcoinStream stream)
		{
			stream.ReadWrite(ref version);
			stream.ReadWrite(ref blockLocators);
			stream.ReadWrite(ref hashStop);
		}
	}
}