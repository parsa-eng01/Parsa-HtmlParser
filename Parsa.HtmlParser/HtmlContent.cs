using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsa.HtmlParser
{
    public class HtmlContent : List<HtmlNode>, IHtmlNode
    {
        public HtmlContent()
        {

        }

        public HtmlContent(IEnumerable<HtmlNode> htmlNodes) : base(htmlNodes)
        {

        }

        public HtmlContent this[string selector] 
        {
            get
            {
                if (!this.Any())
                    return null;

                return null;
                 //this.FirstOrDefault(n => n.TagName == tagName);
            }
        }

        HtmlContent IHtmlNode.this[string selector] => throw new NotImplementedException();

        public string Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string TagName => throw new NotImplementedException();

        public string InnerHtml => throw new NotImplementedException();

        public string InnerText => throw new NotImplementedException();

        public HtmlContent Content { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public HtmlAttributes Attributes => throw new NotImplementedException();

        public HtmlStyle Style => throw new NotImplementedException();

        public bool IsClosed => throw new NotImplementedException();

        public HtmlNode GetElementById(string id)
        {
            throw new NotImplementedException();
        }

        public bool IsValid()
        {
            throw new NotImplementedException();
        }
    }
}
