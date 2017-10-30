using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZeroFormatter;

namespace GraphSerialization.Tests
{
	[TestClass]
	public class ZeroFormatterTests
	{
		private const string simpleString = "Simple string";
		private string singleTtlString;
		private MemoryStream simpleStringMemoryStream = new MemoryStream();
		private MemoryStream singleTtlMemoryStream = new MemoryStream();
		private MemoryStream ttl10000MemoryStream = new MemoryStream();
		private Stopwatch stopWatch;
		private object deserializedSimpleString;
		private object deserializedSingleTtl;
		private static TestResultsLogger logger;

		[TestMethod]
		public void ZeroFormatter_SerializeSimpleString()
		{
			logger.LogLine(String.Format("Serialization of simpleString in {0} miliseconds.", stopWatch.GetMiliseconds(() => ZeroFormatterSerializer.Serialize(simpleStringMemoryStream, simpleString))));
			logger.LogLine(String.Format("Serialized simpleString stream length is {0} bytes.", simpleStringMemoryStream.Length));
			Assert.IsNotNull(simpleStringMemoryStream);
			Assert.AreNotEqual(simpleStringMemoryStream.Length, 0);

			simpleStringMemoryStream.Position = 0;
			logger.LogLine(String.Format("Deserialization of simpleString from stream in {0} miliseconds.", stopWatch.GetMiliseconds(() => this.deserializedSimpleString = ZeroFormatterSerializer.Deserialize<string>(simpleStringMemoryStream))));
			Assert.AreEqual(simpleString, deserializedSimpleString);
		}

		[TestMethod]
		public void ZeroFormatter_SerializeSingleTtl()
		{
			logger.LogLine(String.Format("Serialization of singleTtl in {0} miliseconds.", stopWatch.GetMiliseconds(() => ZeroFormatterSerializer.Serialize(singleTtlMemoryStream, singleTtlString))));
			logger.LogLine(String.Format("Serialized singleTtl stream length is {0} bytes.", singleTtlMemoryStream.Length));
			Assert.IsNotNull(singleTtlMemoryStream);
			Assert.AreNotEqual(singleTtlMemoryStream.Length, 0);

			singleTtlMemoryStream.Position = 0;
			logger.LogLine(String.Format("Deserialization of singleTtl from stream in {0} miliseconds.", stopWatch.GetMiliseconds(() => this.deserializedSingleTtl = ZeroFormatterSerializer.Deserialize<string>(singleTtlMemoryStream))));
			Assert.AreEqual(singleTtlString, deserializedSingleTtl);
		}

		[TestMethod]
		public void ZeroFormatter_SerializeTtl_10000_times()
		{
			logger.LogLine(String.Format("Serialization of Ttl 10000 times in {0} miliseconds.", stopWatch.GetMiliseconds(() => ZeroFormatterSerializer.Serialize(ttl10000MemoryStream, singleTtlString), 10000)));
			logger.LogLine(String.Format("Serialized 10000 Ttl stream length is {0} bytes.", ttl10000MemoryStream.Length));
			Assert.IsNotNull(ttl10000MemoryStream);
			Assert.AreNotEqual(ttl10000MemoryStream.Length, 0);
		}

		[TestMethod]
		public void ZeroFormatter_SerializeTtl_100000_times()
		{
			logger.LogLine(String.Format("Serialization of Ttl 100000 times in {0} miliseconds.", stopWatch.GetMiliseconds(() => ZeroFormatterSerializer.Serialize(new MemoryStream(), singleTtlString), 100000)));
		}

		[TestInitialize]
		public void TestSetup()
		{
			stopWatch = new Stopwatch();
			singleTtlString = File.ReadAllText("data/2594007XIACKNMUAW223.ttl");
		}

		[ClassInitialize]
		public static void ClassSetup(TestContext context)
		{
			logger = TestResultsLogger.Create(nameof(ZeroFormatterTests));
		}

		[ClassCleanup]
		public static void ClassTeardown()
		{
			logger.Dispose();
		}
	}
}