﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NBitcoin.OpenAsset
{
	internal class NoDuplicateColoredTransactionRepository : IColoredTransactionRepository, ITransactionRepository
	{
		public NoDuplicateColoredTransactionRepository(IColoredTransactionRepository inner)
		{
			if (inner == null)
				throw new ArgumentNullException("inner");
			_Inner = inner;
		}

		private IColoredTransactionRepository _Inner;

		#region IColoredTransactionRepository Members

		public ITransactionRepository Transactions
		{
			get
			{
				return this;
			}
		}

		public Task<ColoredTransaction> GetAsync(uint256 txId)
		{
			return Request("c" + txId.ToString(), () => _Inner.GetAsync(txId));
		}

		public Task PutAsync(uint256 txId, ColoredTransaction tx)
		{
			return _Inner.PutAsync(txId, tx);
		}

		#endregion IColoredTransactionRepository Members

		#region ITransactionRepository Members

		Task<Transaction> ITransactionRepository.GetAsync(uint256 txId)
		{
			return Request("t" + txId.ToString(), () => _Inner.Transactions.GetAsync(txId));
		}

		public Task PutAsync(uint256 txId, Transaction tx)
		{
			return _Inner.Transactions.PutAsync(txId, tx);
		}

		#endregion ITransactionRepository Members

		private Dictionary<string, Task> _Tasks = new Dictionary<string, Task>();
		private ReaderWriterLock @lock = new ReaderWriterLock();

		private Task<T> Request<T>(string key, Func<Task<T>> wrapped)
		{
			Task<T> task = null;
			using (@lock.LockRead())
			{
				task = _Tasks.TryGet(key) as Task<T>;
			}
			if (task != null)
				return task;
			using (@lock.LockWrite())
			{
				task = _Tasks.TryGet(key) as Task<T>;
				if (task != null)
					return task;
				task = wrapped();
				_Tasks.Add(key, task);
			}
			task.ContinueWith((_) =>
			{
				using (@lock.LockWrite())
				{
					_Tasks.Remove(key);
				}
			});
			return task;
		}
	}
}