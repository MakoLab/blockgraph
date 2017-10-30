namespace NBitcoin
{
	/// <summary>Defines common base block functionalities.</summary>
	public interface IBaseBlock
	{
		MerkleNode GetMerkleRoot();

		uint256 GetHash();

		void ReadWrite(byte[] array, int startIndex);

		void UpdateMerkleRoot();

		bool Check();

		bool Check(Consensus consensus);

		bool CheckProofOfWork();

		bool CheckProofOfWork(Consensus consensus);

		bool CheckMerkleRoot();
	}
}