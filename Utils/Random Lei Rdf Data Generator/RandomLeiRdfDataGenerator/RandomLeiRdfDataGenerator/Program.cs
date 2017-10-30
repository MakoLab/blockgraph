using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Ionic.Zip;
using VDS.RDF;
using VDS.RDF.Storage;
using VDS.RDF.Writing;

namespace RandomLeiRdfDataGenerator
{
    internal class Program
    {
        private static int RecordsCount = 10;
        private static int StartIndex = 400000;
        private static int Amount = 100000;
        private static int RetrievalCount = 500000;

        private static void Main(string[] args)
        {
            TryReadRecordsCount(args);
            List<string> leiCodes = new List<string>();
            var counter = 0;
            if (!File.Exists(@"leis.txt"))
            {
                var request = WebRequest.CreateHttp(String.Format(Constants.SolrQueryUrl, RetrievalCount));
                var response = request.GetResponse();
                var stream = response.GetResponseStream();

                using (StreamReader reader = new StreamReader(stream))
                using (var fs = File.OpenWrite(@"leis.txt"))
                using (var sw = new StreamWriter(fs))
                {
                    var line = "";
                    while (line != null && !reader.EndOfStream)
                    {
                        line = reader.ReadLine();
                        if (!String.IsNullOrEmpty(line) && line.Trim().Length == 20)
                        {
                            sw.WriteLine(line);
                        }
                    }
                }
                Console.WriteLine("Retrieved records: {0}", leiCodes.Count);
            }
            using (var fs = File.OpenRead(@"leis.txt"))
            using (var sr = new StreamReader(fs))
            {
                for (var i = 0; i < StartIndex; i++)
                {
                    if (sr.EndOfStream)
                    {
                        break;
                    }
                    sr.ReadLine();
                }
                var line = "";
                counter = 0;
                while (!sr.EndOfStream && counter < Amount)
                {
                    line = sr.ReadLine();
                    leiCodes.Add(line);
                    counter++;
                }
            }
            AllegroGraphConnector allegroGraphConnector = CreateConnector();
            counter = 0;
            using (var fs = File.OpenWrite(@"output.nq"))
            using (var sw = new StreamWriter(fs))
            {
                fs.Seek(0, SeekOrigin.End);
                foreach (var record in leiCodes)
                {
                    GetRecord(record, sw, allegroGraphConnector);
                    counter++;
                    if (counter % 1000 == 0)
                    {
                        Console.Write(String.Format("\rParsed {0} records.", counter));
                        allegroGraphConnector.Dispose();
                        allegroGraphConnector = CreateConnector();
                    }
                }
            }

            //ZipRecords(di);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static AllegroGraphConnector CreateConnector()
        {
            return new AllegroGraphConnector(Constants.TripleStoreUrl, Constants.TripleStoreName, Constants.TripleStoreUser, Constants.TripleStorePassword);
        }

        private static void GetRecord(string leiCode, TextWriter output, AllegroGraphConnector allegroGraphConnector)
        {
            var g = new Graph();
            allegroGraphConnector.LoadGraph(g, "graph://lei.info/" + leiCode);
            var ts = new TripleStore();
            ts.Add(g);
            var nqw = new NQuadsWriter();
            var s = VDS.RDF.Writing.StringWriter.Write(ts, nqw);
            output.Write(s);
            output.Flush();
            ts.Remove(g.BaseUri);
            g.Clear();
            ts.Dispose();
            g.Dispose();
        }

        private static void ZipRecords(DirectoryInfo directory)
        {
            using (ZipFile zip = new ZipFile())
            {
                zip.AddFiles(directory.GetFiles().Select(s => s.FullName), String.Empty);
                zip.Save(String.Format("{0}\\lei sample ({1}).zip", directory.FullName, RecordsCount));
                Console.WriteLine("Created zip file: lei sample ({0}).zip", RecordsCount);
            }
        }

        private static void TryReadRecordsCount(string[] args)
        {
            if (args.Length > 0 && args.Contains("-fetch"))
            {
                for (int i = 0; i < args.Length - 1; i++)
                {
                    if (args[i] == "-fetch")
                    {
                        var fetchRaw = args[i + 1];
                        int.TryParse(fetchRaw, out RetrievalCount);
                    }
                }
            }

            if (args.Length > 0 && args.Contains("-records"))
            {
                for (int i = 0; i < args.Length - 1; i++)
                {
                    if (args[i] == "-records")
                    {
                        var recordsRaw = args[i + 1];
                        if (recordsRaw.Equals("all", StringComparison.OrdinalIgnoreCase))
                        {
                            RecordsCount = int.MaxValue;
                        }
                        else
                        {
                            int.TryParse(recordsRaw, out RecordsCount);
                        }
                    }
                }
            }
        }
    }
}