#if !NOJSONNET

using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NBitcoin.RPC
{
	internal abstract class RawFormatter<T> where T : IBlockContent
	{
		protected RawFormatter()
		{
			Network = Network.Main;
		}

		public Network Network
		{
			get;
			set;
		}

		public T ParseJson(string str)
		{
			JObject obj = JObject.Parse(str);
			return Parse(obj);
		}

		[Obsolete("Use RawFormatter.ParseJson method instead")]
		public T Parse(string str)
		{
			JObject obj = JObject.Parse(str);
			return Parse(obj);
		}

		public abstract T Parse(JObject obj);

		protected void WritePropertyValue<TValue>(JsonWriter writer, string name, TValue value)
		{
			writer.WritePropertyName(name);
			writer.WriteValue(value);
		}

		protected abstract void BuildContent(JObject json, T content);

		public string ToString(T content)
		{
			var strWriter = new StringWriter();
			var jsonWriter = new JsonTextWriter(strWriter);
			jsonWriter.Formatting = Formatting.Indented;
			jsonWriter.WriteStartObject();
			WriteContent(jsonWriter, content);
			jsonWriter.WriteEndObject();
			jsonWriter.Flush();
			return strWriter.ToString();
		}

		protected abstract void WriteContent(JsonTextWriter writer, T content);
	}
}

#endif