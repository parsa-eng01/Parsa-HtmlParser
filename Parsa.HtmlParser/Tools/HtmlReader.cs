using Parsa.HtmlParser.HtmlTags;
using Parsa.HtmlParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HtmlParser.HtmlTags;

namespace Parsa.HtmlParser.Tools
{
    public class HtmlReader
    {
        public HtmlReader(string file)
        {
            _file = file;
        }

        enum ReadElement
        {
            None,
            Tag,
            ClosedTag,
            PlainText,
        }

        private string _file;
        private int _readerPosition;


        public HtmlDocument Read()
        {
            if (!File.Exists(_file))
                return null;
            string htmlString = null;
            try
            {
                htmlString = File.ReadAllText(_file, Encoding.UTF8);
            }
            catch { }

            if (string.IsNullOrEmpty(htmlString))
                return null;

            if (!IsHtmlDocument(htmlString))
                return null;

            var head = ParseHead(htmlString);
            if (head == null)
                return null;

            var doc = new HtmlDocument();
            doc.Head = head;
            doc.Body = ParseBody(htmlString);

            return doc;
        }

        private HtmlHead ParseHead(string htmlString)
        {
            HtmlHead head = null;
            var readElement = ReadElement.None;
            var htmlBuffer = string.Empty;
            var isTitle = false;

            for (; _readerPosition < htmlString.Length; _readerPosition++)
            {
                var chr = htmlString[_readerPosition];
                switch (readElement)
                {
                    case ReadElement.None:
                        if (IsIgnoredCharacter(chr))
                            continue;
                        htmlBuffer += chr.ToString();
                        if (chr == '<')
                            readElement = ReadElement.Tag;
                        else
                            readElement = ReadElement.PlainText;
                        break;
                    case ReadElement.Tag:
                        if (htmlBuffer == "<" && char.IsWhiteSpace(chr))
                            continue;

                        htmlBuffer += chr.ToString();
                        if (chr == '>')
                        {
                            if (htmlBuffer.ToLower().StartsWith("<head"))
                                head = new HtmlHead();
                            if (htmlBuffer.ToLower().StartsWith("<title"))
                                isTitle = true;
                            htmlBuffer = string.Empty;
                            readElement = ReadElement.None;
                        }
                        if (chr == '/')
                        {
                            readElement = ReadElement.ClosedTag;
                        }
                        break;
                    case ReadElement.ClosedTag:
                        htmlBuffer += chr.ToString(); 
                        if (chr == '>')
                        {
                            if (htmlBuffer.ToLower() == "</head>")
                            {
                                _readerPosition++;
                                return head;
                            }
                            htmlBuffer = string.Empty;
                            readElement = ReadElement.None;
                        }
                        break;
                    case ReadElement.PlainText:
                        htmlBuffer += chr.ToString();
                        if (chr == '<')
                        {
                            if (isTitle)
                            {
                                isTitle = false;
                                head.Title = htmlBuffer.Remove(htmlBuffer.Length - 1);
                            }
                            htmlBuffer = "<";
                            readElement = ReadElement.Tag;
                        }
                        break;
                }
            }

            return head;
        }

        private HtmlBody ParseBody(string htmlString)
        {
            var readElement = ReadElement.None;
            var htmlBuffer = string.Empty;
            var openTags = new List<HtmlNode>();

            for (; _readerPosition < htmlString.Length; _readerPosition++)
            {
                var chr = htmlString[_readerPosition];
                switch (readElement)
                {
                    case ReadElement.None:
                        if (IsIgnoredCharacter(chr))
                            continue;
                        htmlBuffer += chr.ToString();
                        if (chr == '<')
                            readElement = ReadElement.Tag;
                        else
                            readElement = ReadElement.PlainText;
                        break;
                    case ReadElement.Tag:
                        if (htmlBuffer == "<" && char.IsWhiteSpace(chr))
                            continue;

                        htmlBuffer += chr.ToString();
                        if (chr == '>')
                        {
                            var node = new HtmlNode(htmlBuffer);
                            node.IsClosed = HtmlCondition.EmptyTags.Any(e => e == node.TagName);
                            if (node.TagName == "body")
                                openTags.Add(new HtmlBody(htmlBuffer));
                            else if (htmlBuffer.StartsWith("<!--"))
                                openTags.LastOrDefault()?.Content.Add(new Comment(htmlBuffer));
                            else
                            {
                                AddToParentContent(openTags, node);
                                if (!node.IsClosed)
                                    openTags.Add(node);
                            }
                            htmlBuffer = string.Empty;
                            readElement = ReadElement.None;
                        }
                        if (chr == '/')
                            readElement = ReadElement.ClosedTag;
                        break;
                    case ReadElement.ClosedTag:
                        htmlBuffer += chr.ToString();
                        if (chr == '>')
                        {
                            var tagName = htmlBuffer.Replace("</", "").Replace(">", "");
                            var node = openTags.Last(t => t.TagName == tagName && !t.IsClosed);
                            node.IsClosed = true;
                            CheckUnclosedTags(openTags);

                            if (node is HtmlBody)
                                return node as HtmlBody;
                            htmlBuffer = string.Empty;
                            readElement = ReadElement.None;
                        }
                        break;
                    case ReadElement.PlainText:
                        htmlBuffer += chr.ToString();
                        if (chr == '<')
                        {
                            var node = new PlainText(htmlBuffer.Remove(htmlBuffer.Length - 1));
                            openTags.Last().Content.Add(node);
                            htmlBuffer = "<";
                            readElement = ReadElement.Tag;
                        }
                        break;
                }
                if (openTags.FirstOrDefault(t => t is HtmlBody)?.IsClosed == true)
                    break;
            }

            return openTags.First(t => t is HtmlBody) as HtmlBody;
        }

        private void AddToParentContent(List<HtmlNode> openTags, HtmlNode node)
        {
            var lastNode = openTags.LastOrDefault();
            if (lastNode == null)
                return;

            var parents = new string[] { };
            if (node.TagName == "tr")
                parents = new string[] { "table", "tbody", "thead", "tfoot" };

            if (node.TagName == "td")
                parents = new string[] { "tr", "th" };

            if (new string[] { "tbody", "thead", "tfoot" }.Any(t => t == node.TagName))
                parents = new string[] { "table" };

            if (node.TagName == "il")
                parents = new string[] { "ol", "ul", "menu" };

            if (!parents.Any())
            {
                lastNode.Content.Add(node);
                return;
            }

            for (int i = openTags.Count - 1; i > -1; i--)
            {
                if (parents.Any(t => t == openTags[i].TagName))
                {
                    openTags[i].Content.Add(node);
                    return;
                }
            }
        }

        private List<HtmlNode> CheckUnclosedTags(List<HtmlNode> openTags)
        {
            if (openTags.LastOrDefault()?.IsClosed != false)
            {
                openTags.Remove(openTags.Last());
                return openTags;
            }

            var closedNode = openTags.Last(t => t.IsClosed);
            if (openTags.Last().Content.Count > 1)
            {
                var contents = openTags.Last().Content.Skip(1).ToList();
                openTags.Last().Content.RemoveRange(1, contents.Count);
                closedNode.Content.AddRange(contents);
            }
            var index = openTags.IndexOf(closedNode);
            openTags.RemoveRange(index, openTags.Count - index);

            return openTags;
        }

        private bool IsHtmlDocument(string htmlString)
        {
            if (!htmlString.TrimStart().ToUpper().StartsWith("<!DOCTYPE"))
                return false;

            _readerPosition = htmlString.IndexOf(">") + 1;
            return true;
        }

        private bool IsIgnoredCharacter(char chr)
        {
            if (char.IsWhiteSpace(chr) || chr == '\r' || chr == '\n')
                return true;

            return false;
        }
    }
}
