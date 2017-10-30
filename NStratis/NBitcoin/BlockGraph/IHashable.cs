namespace NBitcoin
{
	/// <summary>Defines the <![CDATA[hashable]]> object interface.</summary>
	public interface IHashable
	{
		/// <summary>Gets the object's hash as <see cref="uint256"/>.</summary>
		uint256 GetHash();
	}
}