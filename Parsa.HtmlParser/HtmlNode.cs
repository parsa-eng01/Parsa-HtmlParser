using Parsa.HtmlParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlParser
{
    public class HtmlNode
    {
        public HtmlNode(string htmlTag)
        {
            _htmlTag = htmlTag.Trim();

            if (!string.IsNullOrEmpty(_tagName) || !htmlTag.StartsWith("<") || htmlTag.StartsWith("<!--"))
                return;

            var tag = htmlTag
                .Replace("<", "")
                .Replace(">", "")
                .Trim()
                .Split(' ')[0]
                .ToLower();

            if (string.IsNullOrEmpty(tag))
                throw new ArgumentException("html tag is not valid");

            Initial(htmlTag, tag);
        }

        protected string _htmlTag;
        protected string _tagName;

        public string TagName => _tagName;
        public string Id 
        {
            get => Attributes["id"];
            protected set => Attributes["id"] = value;
        }
        public virtual string InnerHtml =>
            $"{Build()}\r\n  {InnerText}\r\n</{_tagName}>";
        public virtual string InnerText {
            get 
            {
                if (!_htmlTag.StartsWith("<"))
                    return _htmlTag;

                return Content != null ? string.Join("\r\n  ", Content.Select(c => c.InnerHtml)) : string.Empty;
            }
        }
        public bool IsClosed { get; set; }
        public List<HtmlNode> Content { get; set; } = new List<HtmlNode>();
        public HtmlAttributes Attributes { get; set; } 
        public HtmlStyle Style { get; set; }


        public HtmlNode GetElementById(string id)
        {
            if (Id == id)
                return this;

            if (Content == null)
                return null;

            foreach (var node in Content)
            {
                if (node.GetElementById(id) != null)
                    return node.GetElementById(id);
            }

            return null;
        }

        public virtual bool IsValid()
        {
            if (!IsClosed)
                return false;
            if (Content?.Any(c => !c.IsValid()) == true)
                return false;
                
            return true;
        }


        private Task Initial(string htmlTag, string tag)
        {
            _tagName = tag;
            Attributes = new HtmlAttributes(htmlTag.Remove(0, tag.Length + 1).Replace(">", "").Trim());
            Style = new HtmlStyle(Attributes["style"]);
            
            return Task.CompletedTask;
        }

        private string Build()
        {
            Attributes["style"] = Style.ToString();
            if (string.IsNullOrEmpty(Attributes["style"]))
                Attributes.Remove("style");

            return $"<{TagName} {Attributes}>";
        }




        public override string ToString()
            => _tagName;
    }
}
