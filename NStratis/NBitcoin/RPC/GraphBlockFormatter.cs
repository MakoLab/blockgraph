using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NBitcoin.RPC
{
	/// <summary>Describes the graph block formatter class.</summary>
	internal class GraphBlockFormatter : RawFormatter<GraphContent>
	{
		public override GraphContent Parse(JObject obj)
		{
			GraphContent graph = new GraphContent();
			BuildContent(obj, graph);
			return graph;
		}

		protected override void BuildContent(JObject json, GraphContent content)
		{
			//+ WKDO: Implement proper method content.
			content.Version = (uint)json.GetValue("ver");
			content.Headers = ((object)json.GetValue("header")) as GraphContentHeaders;
			content.Graph = ((object)json.GetValue("graph")) as byte[];
		}

		protected override void WriteContent(JsonTextWriter writer, GraphContent content)
		{
			//+ WKDO: Implement proper method content.
			WritePropertyValue(writer, "hash", content.GetHash().ToString());
			WritePropertyValue(writer, "ver", content.Version);

			WritePropertyValue(writer, "header", content.Headers);
			WritePropertyValue(writer, "graph", content.Graph);
		}
	}
}