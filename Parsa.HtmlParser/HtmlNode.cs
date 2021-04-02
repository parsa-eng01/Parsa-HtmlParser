using Parsa.HtmlParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsa.HtmlParser
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

            _tagName = tag;
        }

        protected string _htmlTag;
        protected string _tagName;

        private Lazy<HtmlAttributes> _attributes;
        private Lazy<HtmlStyle> _style;

        public string Id
        {
            get => Attributes["id"];
            protected set => Attributes["id"] = value;
        }
        public string TagName => _tagName;
        public bool IsClosed { get; set; }
        public HtmlContent Content { get; set; } = new HtmlContent();
        public HtmlAttributes Attributes =>
            _attributes?.Value ??
            (_attributes = new Lazy<HtmlAttributes>(() =>
            {
                return new HtmlAttributes(_htmlTag.Remove(0, _tagName.Length + 1).Replace(">", "").Trim());
            })).Value;
        public HtmlStyle Style =>
            _style?.Value ?? (_style = new Lazy<HtmlStyle>(() =>
            {
                return new HtmlStyle(Attributes["style"]);
            })).Value;

        public virtual string InnerHtml =>
            HtmlCondition.EmptyTags.Any(t => t == _tagName) ? Build() : $"{Build()}\r\n  {InnerText}\r\n</{_tagName}>";

        public virtual string InnerText
        {
            get
            {
                if (!_htmlTag.StartsWith("<"))
                    return _htmlTag;

                return Content != null ? string.Join("\r\n  ", Content.Select(c => c.InnerHtml)) : string.Empty;
            }
        }

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

        private string Build()
        {
            Attributes["style"] = Style.ToString();
            if (string.IsNullOrEmpty(Attributes["style"]))
                Attributes.Remove("style");
            
            if (string.IsNullOrEmpty(Attributes.ToString()))
                return $"<{TagName}>";

            return $"<{TagName} {Attributes}>";
        }




        public override string ToString()
            => _tagName + (!string.IsNullOrEmpty(Id) ? $" id={Id}" : "");
    }
}
