using System.IO;

namespace AdvancedText
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 2)
			{
				var sourceDirectory = args[0];
				if (!Directory.Exists(sourceDirectory))
					return;

				var reportFile = args[1];

				var extractor = new StringExtractor("{", "}");

				using var writer = File.CreateText(reportFile);
				foreach (var filePath in Directory.EnumerateFiles(sourceDirectory, "*.txt", SearchOption.AllDirectories))
				{
					foreach (var line in File.ReadLines(filePath))
						foreach (var item in extractor.Extract(line))
							writer.WriteLine($"{filePath}\t{item}");
				}
			}
		}
	}
}
