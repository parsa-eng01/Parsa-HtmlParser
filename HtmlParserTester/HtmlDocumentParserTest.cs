using HtmlParser;
using Parsa.HtmlParser.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HtmlParserTester
{
    public class HtmlDocumentParserTest
    {

        string[] HtmlFiles = new[]
        {
            @"D:\Projects\Parsa Public Projects\Document\html files\True Trend v1.07 - 2020 Oct 19-True Trend v 1.07_TT Strategy 1 - Oct 22_AUDCAD_H1.htm",
            @"D:\Projects\Parsa Public Projects\Document\html files\True Trend v1.07 - 2020 Oct 19-True Trend v 1.07_TT Strategy 1 - Oct 22_AUDNZD_H1.htm",
        };


        [Fact]
        public void Parse()
        {
            var doc = new HtmlDocument();
            doc.LoadFromFile(HtmlFiles.First());

            Assert.True(doc.Body.Content.Count > 0);
        }

    }
}
