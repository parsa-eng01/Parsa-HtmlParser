using Parsa.HtmlParser.HtmlTags;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Parsa.HtmlParser
{
    public class HtmlDocument
    {
        public HtmlHead Head { get; set; }
        public HtmlBody Body { get; set; }

        public bool IsValid()
            => Head.IsValid() && Body.IsValid();

        public HtmlNode GetElementById(string id)
            => Body.GetElementById(id);

        public override string ToString()
            => Head.Title;
    }
}
