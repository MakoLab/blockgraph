﻿using System.Collections.Generic;

namespace NBitcoin.Protocol
{
	[Payload("blocktxn")]
	public class BlockTxnPayload : Payload
	{
		private uint256 _BlockId;

		public uint256 BlockId
		{
			get
			{
				return _BlockId;
			}
			set
			{
				_BlockId = value;
			}
		}

		private List<Transaction> _Transactions = new List<Transaction>();

		public List<Transaction> Transactions
		{
			get
			{
				return _Transactions;
			}
		}

		public override void ReadWriteCore(BitcoinStream stream)
		{
			stream.ReadWrite(ref _BlockId);
			stream.ReadWrite(ref _Transactions);
		}
	}
}