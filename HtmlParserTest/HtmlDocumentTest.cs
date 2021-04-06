using Parsa.HtmlParser;
using Parsa.HtmlParser.Extensions;
using Parsa.HtmlParser.Tools;
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
            @"D:\Projects\Parsa Public Projects\Parsa-HtmlParser\HtmlParserTest\HtmlFiles\MT4 File1.htm",
            @"D:\Projects\Parsa Public Projects\Parsa-HtmlParser\HtmlParserTest\HtmlFiles\MT4 File2.htm"
        };

        [Fact]
        public void LoadFromFile1x()
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(HtmlFiles.First());

            Assert.True(doc.DocumentNode.ChildNodes.Count > 0);
        }

        [Fact]
        public void LoadFromFile1()
        {
            //var doc = new HtmlDocument();
            //doc.LoadFromFile(HtmlFiles.First());
            var reader = new HtmlReader(HtmlFiles.First());
            var doc = reader.Read();

            Assert.True(doc.Content[0].Content.Count == 8);
        }

        [Fact]
        public void LoadFromFile2()
        {
            var reader = new HtmlReader(HtmlFiles.Last());
            var doc = reader.Read();

            Assert.True(doc.Content[0].Content.Count == 8);
        }


    }
}
