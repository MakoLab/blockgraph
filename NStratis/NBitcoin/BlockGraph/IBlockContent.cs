namespace NBitcoin
{
	/// <summary>Defines the custom graph content interface.</summary>
	public interface IBlockContent : IBitcoinSerializable, IHashable
	{
		uint Version { get; set; }
	}
}