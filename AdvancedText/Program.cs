using System;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

namespace AdvancedText
{
	class Program
	{
		private const string UsageMessage = "Arguments usage: directory rulesFile resultFile" + "\n\r" +
			@"Example: C:\temp\Project C:\temp\rules.json C:\temp\result.txt";

		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				var directory = ReadValidAgrument("directory", v => $"{v} is invalid value", v => Directory.Exists(v));
				var rulesFile = ReadValidAgrument("rules file", v => $"{v} is invalid value", v => File.Exists(v));
				var resultFile = ReadValidAgrument("result file", v => $"{v} is invalid value", v => true);
				Execute(directory, rulesFile, resultFile);
			}
			else if (args.Length == 3)
			{
				Execute(args[0], args[1], args[2]);
			}
			else
			{
				Console.WriteLine(UsageMessage);
			}
			Console.WriteLine("Done");
			Console.ReadKey();
		}

		private static string ReadValidAgrument(string argumentName, Func<string, string> errorMessage, Func<string, bool> validator)
		{
			Console.WriteLine($"Enter {argumentName}:");
			var value = Console.ReadLine();
			while (!validator(value))
			{
				Console.WriteLine(errorMessage(value));
				Console.WriteLine($"Enter {argumentName}:");
				value = Console.ReadLine();
			}
			return value;
		}

		private static void Execute(string directory, string rulesFile, string resultFile)
		{
			try
			{
				var rules = JsonConvert.DeserializeObject<ExtractRule[]>(File.ReadAllText(rulesFile));
				var extractors = rules.Select(r => new { r.Extension, Extractor = new StringExtractor(r.Begin, r.End) }).ToLookup(e => e.Extension, e => e.Extractor);
				using (var writer = File.CreateText(resultFile))
					foreach (var filePath in rules.Select(e => $"*{e.Extension}").Distinct().SelectMany(ext => Directory.EnumerateFiles(directory, ext, SearchOption.AllDirectories)))
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
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
				throw;
			}
		}
	}
}
