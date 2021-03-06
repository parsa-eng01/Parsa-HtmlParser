using HtmlParser.HtmlTags;
using Parsa.HtmlParser.HtmlTags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parsa.HtmlParser
{
    public class HtmlNode : IHtmlNode
    {
        public HtmlNode(string htmlTag)
        {
            _htmlTag = htmlTag;
            if (_htmlTag?.StartsWith("<") != true)
                return;
            if (_htmlTag.StartsWith("<!--"))
                return;

            _tagName = htmlTag
                .Remove(htmlTag.Length - 1)
                .Remove(0, 1)
                .TrimStart();
            if (_tagName.Contains(" "))
                _tagName = _tagName.Substring(0, _tagName.IndexOf(' ')).ToLower();

            if (_tagName.Length == 0)
                throw new ArgumentException("html tag is not valid");

            Content = new HtmlContent();
        }

        protected string _htmlTag;
        protected string _tagName;

        private HtmlAttributes _attributes;
        private HtmlStyle _style;

        public string Id
        {
            get => Attributes.ContainsKey("id") ? Attributes["id"] : null;
            set => Attributes["id"] = value;
        }
        public string TagName => _tagName;
        public bool IsClosed { get; set; }
        public HtmlContent Content { get; set; }
        public HtmlAttributes Attributes =>
            _attributes ?? (_attributes = this.GetType() == typeof(PlainText) || this.GetType() == typeof(Comment) ?
                new HtmlAttributes(string.Empty) :
                new HtmlAttributes(_htmlTag.Remove(0, _tagName.Length + 1).Replace(">", "").Trim()));
        public HtmlStyle Style =>
            _style ?? (_style = this.GetType() == typeof(PlainText) || this.GetType() == typeof(Comment) ?
                new HtmlStyle(string.Empty) :
                new HtmlStyle(Attributes.ContainsKey("style") ? Attributes["style"] : null));

        public virtual string InnerHtml =>
            HtmlCondition.EmptyTags.Any(t => t == _tagName) ? Build() : $"{Build()}\r\n  {InnerText}\r\n</{_tagName}>";

        public virtual string InnerText =>
            Content != null ? string.Join("\r\n  ", Content.Select(c => c.InnerHtml).ToList()) : string.Empty;

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
            if (TagName.Equals(selector, StringComparison.OrdinalIgnoreCase))
                nodes.Add(this);

            if (Content == null)
                return nodes;

            foreach (var node in Content)
                nodes.AddRange(node[selector]);

            return nodes;
        }

        private List<HtmlNode> GetElementsByClass(string selector)
        {
            var nodes = new List<HtmlNode>();
            if (Attributes.ContainsKey("class"))
                if (Attributes["class"].Split(' ').Any(c => c == selector))
                    nodes.Add(this);

            if (Content == null)
                return nodes;

            foreach (var node in Content)
                nodes.AddRange(node["." + selector]);

            return nodes;
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
            => _tagName;// + (!string.IsNullOrEmpty(Id) ? $" id={Id}" : "");
    }
}
