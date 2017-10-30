using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NBitcoin.Utilities;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Writing;
using Xunit;

namespace NBitcoin.Tests
{
	public class graphblock_tests_fixture : IDisposable
	{
		private Stopwatch Stopwatch;
		private TestResultsLogger Logger;

		public graphblock_tests_fixture()
		{
			Stopwatch = new Stopwatch();
			Logger = TestResultsLogger.Create("GraphBlockTests");
		}

		public void StartWatch()
		{
			Stopwatch.Reset();
			Stopwatch.Start();
		}

		public long StopWatch()
		{
			Stopwatch.Stop();
			return Stopwatch.ElapsedMilliseconds;
		}

		public void Log(string formatMessage, string callerName, params object[] parameters)
		{
			if (parameters != null && parameters.Any(p => p == null))
			{
				throw new ArgumentNullException();
			}

			Logger.LogLine(String.Format(formatMessage, parameters), callerName);
		}

		public void Dispose()
		{
			Logger.Dispose();
		}
	}

	/// <summary>Describes graph block tests.</summary>
	public class graphblock_tests : IClassFixture<graphblock_tests_fixture>, IDisposable
	{
		private List<GraphBlock> _smallBlockChain;
		private int _filesCount;
		private TripleStore _smallStore;
		private SparqlResultSet _sparqlResults;
		private Encoding _utf8Encoding;
		private int _graphCount;

		private graphblock_tests_fixture fixture;

		public void SetFixture(graphblock_tests_fixture data)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="graphblock_tests" /> class.</summary>
		public graphblock_tests(graphblock_tests_fixture fixture)
		{
			this.fixture = fixture;

			_smallBlockChain = new List<GraphBlock>();
			_smallStore = new TripleStore();
			_utf8Encoding = Encoding.UTF8;

			// Decide whether to load graphs as serialized ttl strings or serialized IGraph, saved to stream in compressed turtle representation.
			// Choose between LoadBlockChainFromStrings() and LoadBlockChainFromGraphs() and LoadBlockChainFromNQuads()
			LoadBlockChainFromNQuads();
			LoadTripleStore();
			ExecuteSparqlQuery();
		}

		/// <summary>Disposes the instance of the test class.</summary>
		public void Dispose()
		{
		}

		[Fact]
		[Trait("UnitTest", "UnitTest")]
		public void CanCalculateMerkleRoot()
		{
			var graphString = File.ReadAllText(@"data\rdf\2594007XIACKNMUAW223.ttl");
			var graphBytes = _utf8Encoding.GetBytes(graphString);
			GraphBlock graphBlock = new GraphBlock();
			graphBlock.PoAHeader.AuthorityHash = uint256.One;
			GraphContent content = new GraphContent()
			{
				Version = 1,
				Headers = new GraphContentHeaders(),
				Graph = graphBytes
			};
			content.Headers.AddHeader("GraphFormat", "ttl");
			graphBlock.AddGraph(content);

			fixture.StartWatch();
			graphBlock.UpdateMerkleRoot();
			Assert.Equal(graphBlock.Header.HashMerkleRoot, graphBlock.GetMerkleRoot().Hash);
			fixture.Log("Calculated merkle root hash for single graph's block. Elapsed time: {0} miliseconds.", "CanCalculateMerkleRoot", fixture.StopWatch());
		}

		[Fact]
		[Trait("UnitTest", "UnitTest")]
		public void CanCalculateMerkleRoot_Complex()
		{
			GraphBlock graphBlock = new GraphBlock();
			graphBlock.PoAHeader.AuthorityHash = uint256.One;
			foreach (var file in new DirectoryInfo(@"data\rdf\").GetFiles())
			{
				var graphString = File.ReadAllText(file.FullName);
				Encoding utf8Encoding = Encoding.UTF8;
				var graphBytes = utf8Encoding.GetBytes(graphString);
				GraphContent content = new GraphContent()
				{
					Version = 1,
					Headers = new GraphContentHeaders(),
					Graph = graphBytes
				};
				content.Headers.AddHeader("GraphFormat", "ttl");
				graphBlock.AddGraph(content);
			}

			fixture.StartWatch();
			graphBlock.UpdateMerkleRoot();
			Assert.Equal(graphBlock.Header.HashMerkleRoot, graphBlock.GetMerkleRoot().Hash);
			fixture.Log("Calculated merkle root hash for {0} graph's block. Elapsed time: {1} miliseconds.", "CanCalculateMerkleRoot_Complex", graphBlock.Graphs.Count, fixture.StopWatch());
		}

		[Fact]
		[Trait("UnitTest", "UnitTest")]
		public void AreBlocksChained()
		{
			Assert.NotEqual(0, _filesCount);
			Assert.Equal(_smallBlockChain.Count, _filesCount);
		}

		[Fact]
		[Trait("UnitTest", "UnitTest")]
		public void AreNQuadBlocksChained()
		{
			Assert.NotEqual(0, _graphCount);
			Assert.Equal(_smallBlockChain.Count, _graphCount);
		}

		[Fact]
		[Trait("UnitTest", "UnitTest")]
		public void IsTripleStorePopulated()
		{
			Assert.NotEqual(_smallStore.IsEmpty, true);
			Assert.Equal(_smallStore.Graphs.Count(p => p.BaseUri != null), _filesCount);
		}

		[Fact]
		[Trait("UnitTest", "UnitTest")]
		public void QueryHasResults()
		{
			Assert.NotEqual(_sparqlResults.IsEmpty, true);
			Assert.True(_sparqlResults.Count > 0);
			foreach (var result in _sparqlResults.Results)
			{
				Assert.DoesNotMatch(@"/^$|\s+/", ((ILiteralNode)result.Value("name")).Value);
				Assert.IsAssignableFrom(typeof(IUriNode), result.Value("uri"));
				Assert.IsAssignableFrom(typeof(IUriNode), result.Value("country"));
				Assert.Equal("us", ((IUriNode)result.Value("country")).Uri.Segments.Last().ToLowerInvariant());
			}
		}

		[Fact]
		[Trait("UnitTest", "UnitTest")]
		public void IsChainValid()
		{
			fixture.StartWatch();
			for (int i = 0; i < _smallBlockChain.Count; i++)
			{
				Assert.Equal(_smallBlockChain[i].Header.HashPrevBlock, i > 0 ? _smallBlockChain[i - 1].GetHash() : uint256.Zero);
			}

			fixture.Log("Validated existing chain. Elapsed time: {0} miliseconds.", "IsChainValid", fixture.StopWatch());
		}

		[Fact]
		[Trait("UnitTest", "UnitTest")]
		public void IsCompromisedChainInvalid()
		{
			CompromiseBlock(_smallBlockChain[3]);
			var invalidBlockHashesCount = 0;
			fixture.StartWatch();
			for (int i = 0; i < _smallBlockChain.Count; i++)
			{
				if (!_smallBlockChain[i].Header.HashPrevBlock.Equals(i > 0 ? _smallBlockChain[i - 1].GetHash() : uint256.Zero))
				{
					invalidBlockHashesCount++;
				}
			}

			Assert.Equal(invalidBlockHashesCount, 1);
			fixture.Log("Validated existing chain after compromising single block. Found {0} invalid hashes. Elapsed time: {1} miliseconds.", "IsCompromisedChainInvalid", invalidBlockHashesCount, fixture.StopWatch());
		}

		private void LoadBlockChainFromStrings()
		{
			var genesisDate = DateTime.UtcNow;
			var files = new DirectoryInfo(@"data\rdf\").GetFiles("*.ttl", SearchOption.TopDirectoryOnly);
			_filesCount = files.Length;
			fixture.StartWatch();
			foreach (var file in files)
			{
				GraphBlock graphBlock = new GraphBlock();
				graphBlock.PoAHeader.AuthorityHash = uint256.One;
				var graphString = File.ReadAllText(file.FullName);
				Encoding utf8Encoding = Encoding.UTF8;
				var graphBytes = utf8Encoding.GetBytes(graphString);

				GraphContent content = new GraphContent()
				{
					Version = 1,
					Headers = new GraphContentHeaders(),
					Graph = graphBytes
				};
				content.Headers.AddHeader("GraphFormat", "ttl");
				graphBlock.AddGraph(content);
				graphBlock.Header.Nonce = RandomUtils.GetUInt32();
				graphBlock.Header.Version = 1;
				graphBlock.Header.Time = (uint)(DateTime.UtcNow.Ticks - genesisDate.Ticks);
				graphBlock.Header.HashPrevBlock = (_smallBlockChain.LastOrDefault()?.GetHash()) ?? uint256.Zero;
				graphBlock.UpdateMerkleRoot();
				Assert.NotEqual(graphBlock.Header.GetPoWHash(), uint256.Zero);
				Assert.Equal(graphBlock.Header.HashMerkleRoot, graphBlock.GetMerkleRoot().Hash);
				_smallBlockChain.Add(graphBlock);
			}

			fixture.Log("Loaded {0} graphs into {1} blocks. Elapsed time: {2} miliseconds.", "LoadBlockChain", _filesCount, _smallBlockChain.Count, fixture.StopWatch());
		}

		private void LoadBlockChainFromGraphs()
		{
			var genesisDate = DateTime.UtcNow;
			var files = new DirectoryInfo(@"data\rdf\").GetFiles("*.ttl", SearchOption.TopDirectoryOnly);
			_filesCount = files.Length;
			fixture.StartWatch();
			foreach (var file in files)
			{
				GraphBlock graphBlock = new GraphBlock();
				graphBlock.PoAHeader.AuthorityHash = uint256.One;
				var graphString = File.ReadAllText(file.FullName);
				IGraph graph = new Graph();
				graph.BaseUri = new Uri(String.Format("graph://lei.info/{0}", Path.GetFileNameWithoutExtension(file.Name)));
				graph.LoadFromString(graphString, new TurtleParser());
				MemoryStream graphStream = new MemoryStream();
				TextWriter writer = new StreamWriter(graphStream);
				graph.SaveToStream(writer, new CompressingTurtleWriter());
				var graphBytes = graphStream.ToArray();
				GraphContent content = new GraphContent()
				{
					Version = 1,
					Headers = new GraphContentHeaders(),
					Graph = graphBytes
				};
				content.Headers.AddHeader("GraphFormat", "ttl");
				graphBlock.AddGraph(content);
				graphBlock.Header.Nonce = RandomUtils.GetUInt32();
				graphBlock.Header.Version = 1;
				graphBlock.Header.Time = (uint)(DateTime.UtcNow.Ticks - genesisDate.Ticks);
				graphBlock.Header.HashPrevBlock = (_smallBlockChain.LastOrDefault()?.GetHash()) ?? uint256.Zero;
				graphBlock.UpdateMerkleRoot();
				Assert.NotEqual(graphBlock.Header.GetPoWHash(), uint256.Zero);
				Assert.Equal(graphBlock.Header.HashMerkleRoot, graphBlock.GetMerkleRoot().Hash);
				_smallBlockChain.Add(graphBlock);
			}

			fixture.Log("Loaded {0} graphs into {1} blocks. Elapsed time: {2} miliseconds.", "LoadBlockChain", _filesCount, _smallBlockChain.Count, fixture.StopWatch());
		}

		private void LoadBlockChainFromNQuads()
		{
			var genesisDate = DateTime.UtcNow;
			const string FILE_PATH = @"data\rdf\output.nq";
			const int GRAPHS_TO_LOAD_COUNT = 100;
			var sb = new StringBuilder();
			_graphCount = 0;
			var currentGraph = "";
			fixture.StartWatch();
			using (var sr = File.OpenText(FILE_PATH))
			{
				var line = sr.ReadLine();
				sb.Append(line);
				currentGraph = GetGraph(line);
				while (_graphCount < GRAPHS_TO_LOAD_COUNT && !sr.EndOfStream)
				{
					line = sr.ReadLine();
					if (GetGraph(line) != currentGraph)
					{
						var g = CreateGraph(sb.ToString());
						AddGraphToChain(g, genesisDate);
						currentGraph = GetGraph(line);
						sb.Clear();
						_graphCount++;
					}
					sb.Append(line);
				}
			}
			fixture.Log("Loaded {0} graphs into {1} blocks. Elapsed time: {2} miliseconds.", "LoadBlockChain", _graphCount, _smallBlockChain.Count, fixture.StopWatch());
		}

		private IGraph CreateGraph(string s)
		{
			var nqr = new NQuadsParser();
			var ts = new TripleStore();
			nqr.Load(ts, new StringReader(s));
			return ts.Graphs.FirstOrDefault();
		}

		private void LoadTripleStore()
		{
			var turtleParser = new TurtleParser();
			fixture.StartWatch();
			for (int i = 0; i < _smallBlockChain.Count; i++)
			{
				Assert.Equal(_smallBlockChain[i].Header.HashPrevBlock, i > 0 ? _smallBlockChain[i - 1].GetHash() : uint256.Zero);
				Assert.Equal(_smallBlockChain[i].Graphs.Count, 1);
				var graphContent = _smallBlockChain[i].Graphs.First();
				IGraph graph = new Graph();
				graph.LoadFromString(_utf8Encoding.GetString(graphContent.Graph), turtleParser);
				_smallStore.Add(graph);
			}

			fixture.Log("Loaded {0} graphs from {1} blocks into triple store. Elapsed time: {2} miliseconds.", "LoadTripleStore", _smallStore.Graphs.Count(p => p.BaseUri != null), _smallBlockChain.Count, fixture.StopWatch());
		}

		private void ExecuteSparqlQuery()
		{
			fixture.StartWatch();
			ISparqlQueryProcessor processor = new LeviathanQueryProcessor(_smallStore);
			SparqlQueryParser sparqlparser = new SparqlQueryParser();
			SparqlQuery query = sparqlparser.ParseFromString(File.ReadAllText(@"data\rdf\queries\query1.sparql"));
			_sparqlResults = processor.ProcessQuery(query) as SparqlResultSet;
			fixture.Log("Executed sparql query on {0} named graphs yielding {1} results. Elapsed time: {2} miliseconds.", "ExecuteSparqlQuery", _smallStore.Graphs.Count(p => p.BaseUri != null), _sparqlResults.Results.Count, fixture.StopWatch());
		}

		private void CompromiseBlock(GraphBlock block)
		{
			var sourceGraphBytes = block.Graphs.First().Graph;
			var sourceGraphTurtle = _utf8Encoding.GetString(sourceGraphBytes);
			string compromisedGraphTurtle = sourceGraphTurtle.Replace("http://lei.info", "http://lei.inf");
			var compromisedGraphBytes = _utf8Encoding.GetBytes(compromisedGraphTurtle);
			block.Graphs.First().Graph = compromisedGraphBytes;
			block.UpdateMerkleRoot();
		}

		private void AddGraphToChain(IGraph g, DateTime genesisDate)
		{
			GraphBlock graphBlock = new GraphBlock();
			graphBlock.PoAHeader.AuthorityHash = uint256.One;
			Encoding utf8Encoding = Encoding.UTF8;
			byte[] graphBytes;
			using (var s = new MemoryStream())
			using (var sw = new StreamWriter(s))
			{
				g.SaveToStream(sw, new CompressingTurtleWriter());
				graphBytes = s.ToArray();
			}
			GraphContent content = new GraphContent()
			{
				Version = 1,
				Headers = new GraphContentHeaders(),
				Graph = graphBytes
			};
			content.Headers.AddHeader("GraphFormat", "ttl");
			graphBlock.AddGraph(content);
			graphBlock.Header.Nonce = RandomUtils.GetUInt32();
			graphBlock.Header.Version = 1;
			graphBlock.Header.Time = (uint)(DateTime.UtcNow.Ticks - genesisDate.Ticks);
			graphBlock.Header.HashPrevBlock = (_smallBlockChain.LastOrDefault()?.GetHash()) ?? uint256.Zero;
			graphBlock.UpdateMerkleRoot();
			Assert.NotEqual(graphBlock.Header.GetPoWHash(), uint256.Zero);
			Assert.Equal(graphBlock.Header.HashMerkleRoot, graphBlock.GetMerkleRoot().Hash);
			_smallBlockChain.Add(graphBlock);
		}

		private string GetGraph(string line)
		{
			var start = line.LastIndexOf('<');
			var end = line.LastIndexOf('>');
			return line.Substring(start, end - start + 1);
		}
	}
}