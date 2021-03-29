using System;
using System.Collections.Generic;
using System.Text;

namespace HtmlParser.HtmlTags
{
    public class HtmlBody : HtmlNode
    {
        public HtmlBody(string htmlTag) : base(htmlTag)
        {
            TagName = "body";
        }
    }
}
