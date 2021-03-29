using HtmlParser.HtmlTags;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HtmlParser
{
    public class HtmlDocument
    {
        public HtmlHead Head { get; set; }
        public HtmlBody Body { get; set; }

        public bool IsValid()
            => Head.IsValid() && Body.IsValid();

        public HtmlNode GetElementById(string id)
            => Body.GetElementById(id);

        public override string ToString()
            => string.IsNullOrEmpty(Head?.Title) ? "without title" : Head.Title;
    }
}
