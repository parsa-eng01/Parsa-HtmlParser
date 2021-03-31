using HtmlParser.HtmlTags;
using Parsa.HtmlParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HtmlParser.Tools
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
                            if (!htmlBuffer.StartsWith("<!--"))
                            {
                                if (htmlBuffer.Contains("img"))
                                    htmlBuffer = htmlBuffer;
                                var node = new HtmlNode(htmlBuffer);
                                openTags = CheckUnclosedTags(node, openTags);
                                if (node.TagName == "body")
                                    openTags.Add(new HtmlBody(htmlBuffer));
                                else
                                {
                                    openTags.LastOrDefault()?.Content.Add(node);
                                    if (!node.IsClosed)
                                        openTags.Add(node);
                                }
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

                            if (htmlBuffer.ToLower() == "</body>")
                                return openTags.First(t => t is HtmlBody) as HtmlBody;

                            openTags.Remove(node);
                            htmlBuffer = string.Empty;
                            readElement = ReadElement.None;
                        }
                        break;
                    case ReadElement.PlainText:
                        htmlBuffer += chr.ToString();
                        if (chr == '<')
                        {
                            var node = new PlainText(htmlBuffer.Remove(htmlBuffer.Length - 1));
                            openTags.Last(t => !t.IsClosed).Content.Add(node);
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

        private List<HtmlNode> CheckUnclosedTags(HtmlNode node, List<HtmlNode> openTags)
        {
            node.IsClosed = HtmlCondition.EmptyTags.Any(e => e == node.TagName);

            if (HtmlCondition.ParentChildTags.Any(cl => cl.Any(el => el == node.TagName))) 
            {
                var collection = HtmlCondition.ParentChildTags.First(cl => cl.Any(el => el == node.TagName)).ToList();
                var index = collection.IndexOf(node.TagName);
                if (openTags.Any(t => collection.Any(c => c == t.TagName) && collection.IndexOf(t.TagName) >= index && !t.IsClosed)) 
                    openTags
                        .Where(t => collection.Any(c => c == t.TagName) && collection.IndexOf(t.TagName) >= index && !t.IsClosed)
                        .ToList()
                        .ForEach(t => t.IsClosed = true);
            }

            if (openTags.Any(t => t.IsClosed))
            {
                var closedTags = openTags.Where(t => t.IsClosed).ToList();
                for (var i = 0; i < closedTags.Count; i++)
                    openTags.Remove(closedTags[i]); 
            }

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
