using HtmlParser;
using HtmlParser.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsa.HtmlParser.Extensions
{
    public static class HtmlDocumentExtensions
    {
        public static void LoadFromFile(this HtmlDocument htmlDocument, string file)
        {
            var reader = new HtmlReader(file);
            var doc = reader.Read();
            if (doc == null)
                return;

            htmlDocument.Head = doc.Head;
            htmlDocument.Body = doc.Body;
        }
    }
}
