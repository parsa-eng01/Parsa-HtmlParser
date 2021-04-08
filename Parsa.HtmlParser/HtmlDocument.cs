using Parsa.HtmlParser.HtmlTags;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Parsa.HtmlParser
{
    public class HtmlDocument : HtmlNode
    {
        public HtmlDocument(string htmlTag) : base(htmlTag)
        {
        }
        public HtmlDocument():base("<html>")
        {

        }

        public HtmlHead Head 
        {
            get => Content.FirstOrDefault(t => t.TagName == "head") as HtmlHead;
            set
            {
                if (Content.Any(t => t.TagName == "head"))
                    Content.Remove(Content.First(t => t.TagName == "head"));

                Content.Add(value);
            }
        }
        public HtmlBody Body 
        {
            get => Content.FirstOrDefault(t => t.TagName == "body") as HtmlBody;
            set
            {
                if (Content.Any(t => t.TagName == "body"))
                    Content.Remove(Content.First(t => t.TagName == "body"));

                Content.Add(value);
            }
        }

        public override string ToString()
            => Head.Title;
    }
}
