using System;
using System.Diagnostics;
using System.IO;
using BinaryFormatter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphSerialization.Tests
{
	[TestClass]
	public class BinaryConverterTests
	{
		protected BinaryConverter Converter;
		private const string simpleString = "Simple string";
		private string singleTtlString;
		private MemoryStream simpleStringMemoryStream = new MemoryStream();
		private MemoryStream singleTtlMemoryStream = new MemoryStream();
		private MemoryStream ttl10000MemoryStream = new MemoryStream();
		private static Stopwatch stopWatch;
		private object deserializedSimpleString;
		private object deserializedSingleTtl;
		private static TestResultsLogger logger;

		[TestMethod]
		public void BinaryConverter_SerializeSimpleString()
		{
			logger.LogLine(String.Format("Serialization of simpleString in {0} miliseconds.", stopWatch.GetMiliseconds(() => Converter.Serialize(simpleString, simpleStringMemoryStream))));
			logger.LogLine(String.Format("Serialized simpleString stream length is {0} bytes.", simpleStringMemoryStream.Length));
			Assert.IsNotNull(simpleStringMemoryStream);
			Assert.AreNotEqual(simpleStringMemoryStream.Length, 0);

			//simpleStringMemoryStream.Position = 0;
			//logger.LogLine(String.Format("Computing simpleString hash in {0} miliseconds.", stopWatch.GetMiliseconds(() => Hashes.Hash256(simpleStringMemoryStream.ToArray()))));

			simpleStringMemoryStream.Position = 0;
			logger.LogLine(String.Format("Deserialization of simpleString from stream in {0} miliseconds.", stopWatch.GetMiliseconds(() => this.deserializedSimpleString = Converter.Deserialize<string>(simpleStringMemoryStream.ToArray()))));
			Assert.AreEqual(simpleString, deserializedSimpleString);
		}

		[TestMethod]
		public void BinaryConverter_SerializeSingleTtl()
		{
			logger.LogLine(String.Format("Serialization of singleTtl in {0} miliseconds.", stopWatch.GetMiliseconds(() => Converter.Serialize(singleTtlString, singleTtlMemoryStream))));
			logger.LogLine(String.Format("Serialized singleTtl stream length is {0} bytes.", singleTtlMemoryStream.Length));
			Assert.IsNotNull(singleTtlMemoryStream);
			Assert.AreNotEqual(singleTtlMemoryStream.Length, 0);

			//singleTtlMemoryStream.Position = 0;
			//logger.LogLine(String.Format("Computing simpleString hash in {0} miliseconds.", stopWatch.GetMiliseconds(() => Hashes.Hash256(singleTtlMemoryStream.ToArray()))));

			singleTtlMemoryStream.Position = 0;
			logger.LogLine(String.Format("Deserialization of singleTtl from stream in {0} miliseconds.", stopWatch.GetMiliseconds(() => this.deserializedSingleTtl = Converter.Deserialize<string>(singleTtlMemoryStream.ToArray()))));
			Assert.AreEqual(singleTtlString, deserializedSingleTtl);
		}

		[TestMethod]
		public void BinaryConverter_SerializeTtl_10000_times()
		{
			logger.LogLine(String.Format("Serialization of Ttl 10000 times in {0} miliseconds.", stopWatch.GetMiliseconds(() => Converter.Serialize(singleTtlString, ttl10000MemoryStream), 10000)));
			logger.LogLine(String.Format("Serialized 10000 Ttl stream length is {0} bytes.", ttl10000MemoryStream.Length));
			Assert.IsNotNull(ttl10000MemoryStream);
			Assert.AreNotEqual(ttl10000MemoryStream.Length, 0);

			//ttl10000MemoryStream.Position = 0;
			//logger.LogLine(String.Format("Computing simpleString hash in {0} miliseconds.", stopWatch.GetMiliseconds(() => Hashes.Hash256(ttl10000MemoryStream.ToArray()))));
		}

		[TestMethod]
		public void BinaryConverter_SerializeTtl_100000_times()
		{
			logger.LogLine(String.Format("Serialization of Ttl 100000 times in {0} miliseconds.", stopWatch.GetMiliseconds(() => Converter.Serialize(singleTtlString, new MemoryStream()), 100000)));
		}

		[TestInitialize]
		public void TestSetup()
		{
			Converter = new BinaryConverter();
			singleTtlString = File.ReadAllText("data/2594007XIACKNMUAW223.ttl");
		}

		[ClassInitialize]
		public static void ClassSetup(TestContext context)
		{
			stopWatch = new Stopwatch();
			logger = TestResultsLogger.Create(nameof(BinaryConverterTests));
		}

		[ClassCleanup]
		public static void ClassTeardown()
		{
			logger.Dispose();
		}
	}
}