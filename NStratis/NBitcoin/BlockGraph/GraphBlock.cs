using System.Collections.Generic;
using System.IO;
using System.Linq;
using NBitcoin.RPC;
using Newtonsoft.Json.Linq;

namespace NBitcoin
{
	/// <summary>Describes the graph block object.</summary>
	public class GraphBlock : BaseBlock, IBaseBlock, IBitcoinSerializable
	{
		private PoAHeader _poaHeader = new PoAHeader();

		private List<GraphContent> _graphs = new List<GraphContent>();

		/// <summary>Gets or sets the collection of <see cref="GraphContent"/> objects.</summary>
		public List<GraphContent> Graphs
		{
			get { return _graphs; }
			set { _graphs = value; }
		}

		/// <summary>Gets or sets the proof-of-authority header.</summary>
		public PoAHeader PoAHeader
		{
			get { return _poaHeader; }
			set { _poaHeader = value; }
		}

		private BlockHeader header = new BlockHeader();

		public MerkleNode GetMerkleRoot()
		{
			return MerkleNode.GetRoot(_graphs.Select(t => t.GetHash()).Prepend(_poaHeader.GetHash()));
		}

		public GraphBlock()
		{
			SetNull();
		}

		public GraphBlock(BlockHeader blockHeader)
		{
			SetNull();
			header = blockHeader;
		}

		public GraphBlock(byte[] bytes)
		{
			this.ReadWrite(bytes);
		}

		public void ReadWrite(BitcoinStream stream)
		{
			stream.ReadWrite(ref header);
			stream.ReadWrite(ref _poaHeader);
			stream.ReadWrite(ref _graphs);
		}

		public bool HeaderOnly
		{
			get
			{
				return _poaHeader == null || _graphs == null || _graphs.Count == 0;
			}
		}

		private void SetNull()
		{
			header.SetNull();
			_graphs.Clear();
		}

		public BlockHeader Header
		{
			get
			{
				return header;
			}
		}

		public uint256 GetHash()
		{
			//Block's hash is his header's hash
			return header.GetHash();
		}

		public void ReadWrite(byte[] array, int startIndex)
		{
			var ms = new MemoryStream(array);
			ms.Position += startIndex;
			BitcoinStream bitStream = new BitcoinStream(ms, false);
			ReadWrite(bitStream);
		}

		public GraphContent AddGraph(GraphContent graph)
		{
			Graphs.Add(graph);
			return graph;
		}

		///// <summary>
		///// Create a block with the specified option only. (useful for stripping data from a block)
		///// </summary>
		///// <param name="options">Options to keep</param>
		///// <returns>A new block with only the options wanted</returns>
		//public GraphBlock WithOptions(TransactionOptions options)
		//{
		//	if (Graphs.Count == 0)
		//		return this;
		//	var instance = new GraphBlock();
		//	var ms = new MemoryStream();
		//	var bms = new BitcoinStream(ms, true);
		//	bms.TransactionOptions = options;
		//	this.ReadWrite(bms);
		//	ms.Position = 0;
		//	bms = new BitcoinStream(ms, false);
		//	bms.TransactionOptions = options;
		//	instance.ReadWrite(bms);
		//	return instance;
		//}

		public void UpdateMerkleRoot()
		{
			this.Header.HashMerkleRoot = GetMerkleRoot().Hash;
		}

		/// <summary>
		/// Check proof of work and merkle root
		/// </summary>
		/// <returns></returns>
		public bool Check()
		{
			return Check(null);
		}

		/// <summary>
		/// Check proof of work and merkle root
		/// </summary>
		/// <param name="consensus"></param>
		/// <returns></returns>
		public bool Check(Consensus consensus)
		{
			//+ WKDO: Investigate whether the below code should remain here (PoS and PoW)? For now, GraphBlock is using PoA by default.
			//if (Block.BlockSignature)
			//	return BlockStake.Check(this);

			// if is proof of authority
			if (true)
			{
				return CheckProofOfAuthority();
			}

			// return CheckMerkleRoot() && Header.CheckProofOfWork(consensus);
		}

		public bool CheckProofOfAuthority()
		{
			//+ WKDO: Current implementation checks only whether the PoA header has any authority hash at all. Needs proper implementation.
			return !PoAHeader.IsEmpty;
		}

		public bool CheckProofOfWork()
		{
			return CheckProofOfWork(null);
		}

		public bool CheckProofOfWork(Consensus consensus)
		{
			return Header.CheckProofOfWork(consensus);
		}

		public bool CheckMerkleRoot()
		{
			return Header.HashMerkleRoot == GetMerkleRoot().Hash;
		}

		//public GraphBlock CreateNextBlockWithCoinbase(BitcoinAddress address, int height)
		//{
		//	return CreateNextBlockWithCoinbase(address, height, DateTimeOffset.UtcNow);
		//}

		//public GraphBlock CreateNextBlockWithCoinbase(BitcoinAddress address, int height, DateTimeOffset now)
		//{
		//	if (address == null)
		//		throw new ArgumentNullException("address");
		//	GraphBlock block = new GraphBlock();
		//	block.Header.Nonce = RandomUtils.GetUInt32();
		//	block.Header.HashPrevBlock = this.GetHash();
		//	block.Header.BlockTime = now;
		//	var graph = block.AddGraph(new GraphContent());
		//	return block;
		//}

		//public GraphBlock CreateNextBlockWithCoinbase(PubKey pubkey, Money value)
		//{
		//	return CreateNextBlockWithCoinbase(pubkey, value, DateTimeOffset.UtcNow);
		//}

		//public GraphBlock CreateNextBlockWithCoinbase(PubKey pubkey, Money value, DateTimeOffset now)
		//{
		//	GraphBlock block = new GraphBlock();
		//	block.Header.Nonce = RandomUtils.GetUInt32();
		//	block.Header.HashPrevBlock = this.GetHash();
		//	block.Header.BlockTime = now;
		//	var graph = block.AddGraph(new GraphContent());

		//	return block;
		//}

#if !NOJSONNET

		public static GraphBlock ParseJson(string json)
		{
			var formatter = new GraphBlockFormatter();
			var block = JObject.Parse(json);
			var graphs = (JArray)block["graphs"];
			var poaHeader = uint256.Parse((string)block["poa_header"]);
			GraphBlock blk = new GraphBlock();
			blk.Header.Bits = new Target((uint)block["bits"]);
			blk.Header.BlockTime = Utils.UnixTimeToDateTime((uint)block["time"]);
			blk.Header.Nonce = (uint)block["nonce"];
			blk.Header.Version = (int)block["ver"];
			blk.Header.HashPrevBlock = uint256.Parse((string)block["prev_block"]);
			blk.Header.HashMerkleRoot = uint256.Parse((string)block["mrkl_root"]);
			blk.PoAHeader = new PoAHeader(poaHeader);
			foreach (var graph in graphs)
			{
				blk.AddGraph(formatter.Parse((JObject)graph));
			}
			return blk;
		}

#endif

		//+ WKDO: Check whether the below code is neccessary in graph block.

		//public static GraphBlock Parse(string hex)
		//{
		//	return new GraphBlock(Encoders.Hex.DecodeData(hex));
		//}

		//public MerkleBlock Filter(params uint256[] txIds)
		//{
		//	return new MerkleBlock(this, txIds);
		//}

		//public MerkleBlock Filter(BloomFilter filter)
		//{
		//	return new MerkleBlock(this, filter);
		//}
	}
}