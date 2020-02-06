using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedText
{
	public class StringExtractor
	{
		private readonly string _begin;
		private readonly string _end;

		public StringExtractor(string begin, string end)
		{
			_begin = begin ?? throw new ArgumentException(nameof(begin));
			_end = end ?? throw new ArgumentException(nameof(end));
		}

		private enum ReadState
		{
			Begin,
			End
		}

		public string[] Extract(string sourceText)
		{
			var state = ReadState.Begin;
			var resultList = new List<string>();
			var midResultvalue = new List<char>();
			var beginMatchDegree = 0;
			var endMatchDegree = 0;
			using (var sourceTextEnumerator = sourceText.GetEnumerator())
			{
				while (sourceTextEnumerator.MoveNext())
				{
					var current = sourceTextEnumerator.Current;
					switch (state)
					{
						case ReadState.Begin:
							if (current == _begin[beginMatchDegree])
							{
								if (++beginMatchDegree == _begin.Length)
								{
									beginMatchDegree = 0;
									state = ReadState.End;
								}
							}
							break;
						case ReadState.End:
							midResultvalue.Add(current);
							if (current == _end[endMatchDegree])
							{
								if (++endMatchDegree == _end.Length)
								{
									endMatchDegree = 0;
									state = ReadState.Begin;
									resultList.Add(new string(midResultvalue.Take(midResultvalue.Count - _end.Length).ToArray()));
									midResultvalue.Clear();
								}
							}
							break;
						default:
							break;
					}
				}
			}
			return resultList.ToArray();
		}
	}
}
