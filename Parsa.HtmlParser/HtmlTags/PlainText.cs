using Parsa.HtmlParser;

namespace HtmlParser.HtmlTags
{
    public class PlainText : HtmlNode
    {
        public PlainText(string htmlTag) : base(null)
        {
            _tagName = "Plain Text";
            _htmlTag = htmlTag;
            IsClosed = true;
        }

        public override string InnerHtml => _htmlTag;
        public override string InnerText => _htmlTag;
    }
}
