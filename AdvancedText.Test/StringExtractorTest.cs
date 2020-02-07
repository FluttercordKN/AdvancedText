using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdvancedText.Test
{
	[TestClass]
	public class StringExtractorTest
	{
		[TestMethod]
		public void Scenario1()
		{
			var instance = new StringExtractor("12", "345");

			var text = "qwert12asd345zxc";

			var result = instance.Extract(text);
			Assert.AreEqual(1, result.Length);
			Assert.AreEqual("asd", result[0]);
		}

		[TestMethod]
		public void Scenario2()
		{
			var instance = new StringExtractor("12", "345");

			var text = "qwert12asd345zxc12rty345vbn";

			var result = instance.Extract(text);
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual("asd", result[0]);
			Assert.AreEqual("rty", result[1]);
		}

		[TestMethod]
		public void CorruptedNoEnd()
		{
			var instance = new StringExtractor("12", "345");

			var text = "qwert12asdzxc";

			var result = instance.Extract(text);
			Assert.AreEqual(0, result.Length);
		}

		[TestMethod]
		public void RealCase()
		{
			var instance = new StringExtractor("@R(\"", "\"");

			var text = "< a rel = \"nofollow\" href = \"@Url.Action(\"history\", \"Resource\", new { id = Model.Id, title = Model.Title.NormalizeFormat(true), area = string.Empty })\" id = \"history\" > @R(\"Show history\") </ a > ";

			var result = instance.Extract(text);
			Assert.AreEqual(1, result.Length);
			Assert.AreEqual("Show history", result[0]);
		}
	}
}
