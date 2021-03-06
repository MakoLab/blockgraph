﻿namespace NBitcoin.Protocol
{
	public class BitcoinSerializablePayload<T> : Payload where T : IBitcoinSerializable, new()
	{
		public BitcoinSerializablePayload()
		{
		}

		public BitcoinSerializablePayload(T obj)
		{
			_Object = obj;
		}

		private T _Object = new T();

		public T Object
		{
			get
			{
				return _Object;
			}
			set
			{
				_Object = value;
			}
		}

		public override void ReadWriteCore(BitcoinStream stream)
		{
			stream.ReadWrite(ref _Object);
		}
	}
}