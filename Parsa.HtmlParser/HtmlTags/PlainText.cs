using System;
using System.Collections.Generic;
using System.Text;

namespace HtmlParser.HtmlTags
{
    public class PlainText : HtmlNode
    {
        public PlainText(string htmlTag) : base(htmlTag)
        {
            TagName = "Plain Text";
        }
    }
}
