namespace Parsa.HtmlParser
{
    public class HtmlHead : HtmlNode
    {
        public HtmlHead(string htmlTag) : base(htmlTag)
        {
        }

        public string Title => Content["title"]?.InnerText;



        public override string ToString()
            => string.IsNullOrEmpty(Title) ? "No title" : Title;
    }
}
