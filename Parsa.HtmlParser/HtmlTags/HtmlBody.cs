using System;
using System.Collections.Generic;
using System.Text;

namespace Parsa.HtmlParser.HtmlTags
{
    public class HtmlBody : HtmlNode
    {
        public HtmlBody(string htmlTag) : base(htmlTag)
        {
            _tagName = "body";
        }
    }
}
