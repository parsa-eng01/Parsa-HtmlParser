namespace Parsa.HtmlParser
{
    public interface IHtmlNode
    {
        string Id { get; set; }
        string TagName { get; }
        string InnerHtml { get; }
        string InnerText { get; }
        HtmlContent Content { get; set; }
        HtmlAttributes Attributes { get; }
        HtmlStyle Style { get; }
        bool IsClosed { get; }

        HtmlContent this[string selector] { get; }


        bool IsValid();
        HtmlNode GetElementById(string id);


    }
}
