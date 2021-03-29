using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlParser
{
    public class HtmlNode
    {
        public HtmlNode(string htmlTag)
        {
            _htmlTag = htmlTag.Trim();

            if (!string.IsNullOrEmpty(TagName) || !htmlTag.StartsWith("<"))
                return;
            var tag = htmlTag
                .Replace("<", "")
                .Trim()
                .Split(' ')[0];

            if (string.IsNullOrEmpty(tag))
                throw new ArgumentException("html tag is not valid");
        }

        private string _htmlTag;
        private HtmlStyle _style;
        private Dictionary<string, string> _attributes;

        protected string TagName;

        public string Id 
        {
            get => GetAttribute("id");
            protected set => SetAttribute("id", value);
        }
        public string InnerHtml =>
            $"{_htmlTag}\r\n  {InnerText}\r\n<\\{TagName}>";
        public string InnerText {
            get 
            {
                if (_htmlTag.StartsWith("<") && _htmlTag.EndsWith(">"))
                    return _htmlTag;

                return Content != null ? string.Join("\r\n  ", Content.Select(c => c.InnerHtml)) : string.Empty;
            }
        }
        public bool IsClosed { get; set; }
        public List<HtmlNode> Content { get; set; }

        public string this[string attribute]
        {
            get => GetAttribute(attribute);
            set => SetAttribute(attribute, value);
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

        public string GetAttribute(string attribute)
        {
            if (_attributes == null)
                _attributes = GetAllAttributeSeparatly();
            if (!_attributes.Any())
                return null;

            return _attributes.ContainsKey(attribute) ? _attributes[attribute] : null;
        }

        public void SetAttribute(string attribute, string value)
        {
            if (_attributes == null)
                _attributes = GetAllAttributeSeparatly();
            if (!_attributes.Any())
                return;

            _attributes[attribute] = value;
            BuildAttributeAsHtmlTag();
        }

        public string GetStyle(string element = null)
        {
            if (_style == null)
                _style = new HtmlStyle(GetAttribute("style"));

            return _style[element];
        }

        public void SetStyle (string element, string value)
        {
            if (string.IsNullOrEmpty(element) || string.IsNullOrEmpty(value))
                throw new ArgumentException("the parameters can not be empty");

            _style[element] = value;
        }

        private Dictionary<string, string> GetAllAttributeSeparatly()
        {
            if (!_htmlTag.Contains("=") && !_htmlTag.StartsWith("<") && !_htmlTag.EndsWith(">")) 
                return new Dictionary<string, string>();

            var tagBuffer = _htmlTag
                .Remove(0, TagName.Length + 1)
                .Replace(">", "");

            var isValue = false;
            var name = string.Empty;
            var value = string.Empty;
            var qoute = '"';
            var attribute = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var chr in tagBuffer)
            {
                if (chr == '=')
                {
                    isValue = true;
                    attribute[name] = string.Empty;
                    value = string.Empty;
                    continue;
                }    

                if (isValue)
                {
                    if (value == string.Empty && char.IsWhiteSpace(chr))
                        continue;

                    if (value == string.Empty && char.IsWhiteSpace(qoute))
                    {
                        qoute = chr;
                        continue;
                    }
                    if (chr == qoute)
                    {
                        qoute = ' ';
                        isValue = false;
                        attribute[name] = value;
                        name = string.Empty;
                        continue;
                    }

                    value += chr;
                }
                else
                {
                    if (char.IsWhiteSpace(chr))
                    {
                        if (!string.IsNullOrEmpty(name))
                        {
                            attribute[name] = string.Empty;
                            name = string.Empty;
                        }
                        continue;
                    }

                    name += chr;
                }
            }

            return attribute;
        }

        private void BuildAttributeAsHtmlTag()
        {
            _htmlTag = $"<{TagName} " +
                string.Join(" ", _attributes.Select(a => $"{a.Key}=\"{a.Value}\"")) +
                $">";
        }




        public override string ToString()
            => TagName;
    }
}
