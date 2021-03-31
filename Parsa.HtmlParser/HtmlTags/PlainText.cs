using System;
using System.Collections.Generic;
using System.Text;

namespace HtmlParser.HtmlTags
{
    public class PlainText : HtmlNode
    {
        public PlainText(string htmlTag) : base(htmlTag)
        {
            _tagName = "Plain Text";
            IsClosed = true;
        }

        public override string InnerHtml() => _htmlTag;
        public override string InnerText() => _htmlTag;
    }
}
