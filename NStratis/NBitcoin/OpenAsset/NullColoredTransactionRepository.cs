﻿using System;
using System.Threading.Tasks;

namespace NBitcoin.OpenAsset
{
	/// <summary>
	/// A colored transaction repository which does not save ColoredTransaction
	/// </summary>
	public class NullColoredTransactionRepository : IColoredTransactionRepository
	{
		private ITransactionRepository _Inner;

		public NullColoredTransactionRepository(ITransactionRepository repo)
		{
			if (repo == null)
				throw new ArgumentNullException("repo");
			_Inner = repo;
		}

		#region IColoredTransactionRepository Members

		public ITransactionRepository Transactions
		{
			get
			{
				return _Inner;
			}
		}

		public Task<ColoredTransaction> GetAsync(uint256 txId)
		{
			return Task.FromResult<ColoredTransaction>(null);
		}

		public Task PutAsync(uint256 txId, ColoredTransaction tx)
		{
			return Task.FromResult<bool>(true);
		}

		#endregion IColoredTransactionRepository Members
	}
}