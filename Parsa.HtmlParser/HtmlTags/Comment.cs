using HtmlParser.HtmlTags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsa.HtmlParser.HtmlTags
{
    public class Comment : HtmlNode
    {
        public Comment(string htmlTag) : base(null)
        {
            _tagName = "comment";
            _htmlTag = htmlTag.Replace("<!--","").Replace("-->","").Trim();
            IsClosed = true;
        }

        public override string InnerHtml => $"<!-- {_htmlTag} -->";
        public override string InnerText => _htmlTag;
    }
}
