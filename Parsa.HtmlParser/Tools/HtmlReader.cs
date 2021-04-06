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

            var doc = Parse(htmlString);

            return doc;
        }

        private HtmlDocument Parse(string htmlString)
        {
            var readElement = ReadElement.None;
            var htmlBuffer = string.Empty;
            var index = _readerPosition;
            var openTags = new List<HtmlNode>();

            for (; _readerPosition < htmlString.Length; _readerPosition++)
            {
                var chr = htmlString[_readerPosition];
                if (readElement == ReadElement.None)
                {
                    if (IsIgnoredCharacter(chr))
                    {
                        index++;
                        continue;
                    }
                    if (chr == '<')
                        readElement = ReadElement.Tag;
                    else
                        readElement = ReadElement.PlainText;
                }
                else if (readElement == ReadElement.Tag && chr == '>')
                {
                    htmlBuffer = "<";
                    for (int i = index + 1; i <= _readerPosition; i++)
                    {
                        if (htmlBuffer.Length == 1 && char.IsWhiteSpace(chr))
                            continue;
                        htmlBuffer += htmlString[i];
                    }
                    

                    if (htmlBuffer.StartsWith("</"))
                    {
                        var tagName = htmlBuffer.Remove(htmlBuffer.Length - 1).Remove(0, 2);
                        var closedNode = openTags.Last(t => t.TagName == tagName);
                        closedNode.IsClosed = true;
                        CheckUnclosedTags(openTags);

                        if (closedNode is HtmlDocument)
                            return closedNode as HtmlDocument;

                        index = _readerPosition + 1;
                        readElement = ReadElement.None;

                        continue;
                    }
                    var node = new HtmlNode(htmlBuffer);
                    node.IsClosed = HtmlCondition.EmptyTags.Any(e => e == node.TagName);
                    if (node.TagName == "html")
                        node = new HtmlDocument(htmlBuffer);
                    else if (node.TagName == "head")
                        node = new HtmlHead(htmlBuffer);
                    else if (node.TagName == "body")
                        node = new HtmlBody(htmlBuffer);
                    else if (htmlBuffer.StartsWith("<!--"))
                        node = new Comment(htmlBuffer);

                    AddToParentContent(openTags, node);
                    if (!node.IsClosed)
                        openTags.Add(node);

                    index = _readerPosition + 1;
                    readElement = ReadElement.None;

                }
                else if(readElement == ReadElement.PlainText && chr == '<')
                {
                    htmlBuffer = htmlString.Substring(index + 1, _readerPosition - index - 1).Trim();
                    var node = new PlainText(htmlBuffer);
                    openTags.Last().Content.Add(node);

                    readElement = ReadElement.Tag;
                    index = _readerPosition;
                }

            }

            return openTags.First() as HtmlDocument;
        }

        private void AddToParentContent(List<HtmlNode> openTags, HtmlNode node)
        {
            if (!openTags.Any())
                return;

            if (!HtmlCondition.HtmlParents.ContainsKey(node.TagName))
            {
                openTags.LastOrDefault().Content.Add(node);
                return;
            }

            var parents = HtmlCondition.HtmlParents[node.TagName];

            for (int i = openTags.Count - 1; i > -1; i--)
            {
                if (parents.Any(t => t == openTags[i].TagName))
                {
                    openTags[i].Content.Add(node);
                    return;
                }
            }
        }

        private void CheckUnclosedTags(List<HtmlNode> openTags)
        {
            if (openTags.LastOrDefault()?.IsClosed != false)
            {
                openTags.Remove(openTags.Last());
                return;
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
