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
            @"D:\Projects\Parsa Public Projects\Parsa-HtmlParser\HtmlParserTest\HtmlFiles\MT4 File3.htm"
        };

        private string HtmlString = "<!DOCTYPE html>" +
                                    "<html>" +
                                    "<head>" +
                                    "<title>Page Title</title>" +
                                    "</head>" +
                                    "<body class='someClass'>" +
                                    "<h1 id='uniqId'>This is a Heading</h1>" +
                                    "<p >This is a paragraph.</p>" +
                                    "</body>" +
                                    "</html>";

        [Fact]
        public void LoadFromFile1()
        {
            var doc = HtmlReader.Read(HtmlFiles.First());
            var tables = doc["table"];
            var head = doc.Head.Title;

            Assert.True(tables.Count == 2);
        }

        [Fact]
        public void LoadFromFile2()
        {
            var doc = HtmlReader.Read(HtmlFiles.Last());
            var tables = doc["table"];

            Assert.True(tables.Count == 2);
            Assert.True(doc.Content[0].Content.Count == 8);
        }

        [Fact]
        public void LoadFromString()
        {
            var doc = HtmlReader.Parse(HtmlString);
            var tables = doc["table"];
            var someCalsses = doc[".someClass"];
            var someId = doc["#uniqId"];
            var p = doc["p"];

            Assert.True(tables.Count == 0);
            Assert.True(someCalsses.Count == 1 && someCalsses.TagName == "body");
            Assert.True(someId.Count == 1 && someId.TagName == "h1");
            Assert.True(p.Count == 1 && p.TagName == "p");
        }

    }
}
