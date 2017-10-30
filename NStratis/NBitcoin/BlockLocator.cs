﻿using System.Collections.Generic;

namespace NBitcoin
{
	/// <summary>
	/// Compact representation of one's chain position which can be used to find forks with another chain
	/// </summary>
	public class BlockLocator : IBitcoinSerializable
	{
		public BlockLocator()
		{
		}

		private List<uint256> vHave = new List<uint256>();

		public List<uint256> Blocks
		{
			get
			{
				return vHave;
			}
			set
			{
				vHave = value;
			}
		}

		#region IBitcoinSerializable Members

		public void ReadWrite(BitcoinStream stream)
		{
			stream.ReadWrite(ref vHave);
		}

		#endregion IBitcoinSerializable Members
	}
}