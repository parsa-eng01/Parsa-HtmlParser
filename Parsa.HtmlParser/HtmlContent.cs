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


        public string Id 
        {
            get => Count == 1 ? this[0].Id : null;
            set { if (Count == 1) this[0].Id = value; }
        }

        public string TagName => Count == 1 ? this[0].TagName : null;

        public string InnerHtml => string.Join("\r\n", this.Select(n => n.InnerHtml));

        public string InnerText => Count == 1 ? this[0].InnerText : null;

        public HtmlContent Content 
        { 
            get => Count == 1 ? this[0].Content : null;
            set { if (Count == 1) this[0].Content = value; }
        }

        public HtmlAttributes Attributes => Count == 1 ? this[0].Attributes : null;

        public HtmlStyle Style => Count == 1 ? this[0].Style : null;

        public bool IsClosed => Count == 1 ? this[0].IsClosed : false;

        public HtmlContent this[string selector]
        {
            get
            {
                if (string.IsNullOrEmpty(selector))
                    return null;
                if (selector.StartsWith("#"))
                    return new HtmlContent { GetElementById(selector.Remove(0, 1)) };
                if (selector.StartsWith("."))
                    return new HtmlContent(GetElementsByClass(selector.Remove(0, 1)));

                return new HtmlContent(GetElementsByTagName(selector));
            }
        }

        private IEnumerable<HtmlNode> GetElementsByTagName(string selector)
        {
            var nodes = new List<HtmlNode>();

            foreach (var node in this)
                nodes.AddRange(node[selector]);

            return nodes;
        }

        private List<HtmlNode> GetElementsByClass(string selector)
        {
            var nodes = new List<HtmlNode>();

            foreach (var node in this)
                nodes.AddRange(node["." + selector]);

            return nodes;
        }
        public HtmlNode GetElementById(string id)
        {
            foreach (var node in this)
            {
                if (node.GetElementById(id) != null)
                    return node.GetElementById(id);
            }

            return null;
        }

        public bool IsValid()
        {
            return !this.Any(n => !n.IsValid());

        }
    }
}
