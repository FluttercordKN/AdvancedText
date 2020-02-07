using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json;

namespace AdvancedText
{
	class Program
	{
		private const string RulesFileName = "ExtractRules.json";
		private const string ReportFileName = "Report.tsv";

		static void Main(string[] args)
		{
			if (args.Length != 1)
				return;

			var sourceDirectory = args[0];
			if (!Directory.Exists(sourceDirectory))
				return;

			try
			{
				var executingAssemblyFolder = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
				var rules = JsonConvert.DeserializeObject<ExtractRule[]>(File.ReadAllText(Path.Combine(executingAssemblyFolder, RulesFileName)));
				var extractors = rules.Select(r => new { r.Extension, Extractor = new StringExtractor(r.Begin, r.End)}).ToLookup(e => e.Extension, e => e.Extractor);
				using (var writer = File.CreateText(Path.Combine(executingAssemblyFolder, ReportFileName)))
					foreach (var filePath in rules.Select(e => $"*{e.Extension}").Distinct().SelectMany(ext => Directory.EnumerateFiles(sourceDirectory, ext, SearchOption.AllDirectories)))
					{
						var fileExtension = new FileInfo(filePath).Extension;
						var lineNumber = 0;
						foreach (var line in File.ReadLines(filePath))
						{
							lineNumber++;
							foreach (var extractor in extractors[fileExtension])
								foreach (var item in extractor.Extract(line))
									writer.WriteLine($"{filePath}:{lineNumber}\t{item}");
						}
					}
				Console.WriteLine("Done");
				Console.ReadKey();
			}
			catch (Exception e)
			{
				Console.WriteLine($"{e.Message}{Environment.NewLine}{e.StackTrace}");
				Console.ReadKey();
			}
		}
	}
}
