using HtmlParser;
using Parsa.HtmlParser.Extensions;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace HtmlParserTest
{
    public class HtmlDocumentTest
    {
        private string[] HtmlFiles = new[]
        {
            @"D:\Projects\Parsa Public Projects\Parsa-HtmlParser\HtmlParserTest\HtmlFiles\MT4 File1.htm"
        };

        [Fact]
        public void LoadFromFile()
        {
            var doc = new HtmlDocument();
            doc.LoadFromFile(HtmlFiles.First());
            var text = doc.Body.InnerHtml;

            Assert.True(doc.Body.Content.Count > 0);
            Assert.True(doc.IsValid());
        }
    }
}
