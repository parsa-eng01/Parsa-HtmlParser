using HtmlParser.HtmlTags;
using Parsa.HtmlParser.HtmlTags;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System;

namespace Parsa.HtmlParser.Tools
{
    public class HtmlReader
    {
        public HtmlReader()
        {

        }

        enum ReadElement
        {
            None,
            Tag,
            PlainText,
        }

        private static int _readerPosition;


        public static HtmlDocument Read(string file)
        {
            var htmlString = File.ReadAllBytes(file);

            if (!htmlString.Any())
                return null;
            _readerPosition = 0;
            if (!IsHtmlDocument(htmlString))
                return null;

            var doc = Parse(htmlString);

            return doc;
        }

        public static HtmlDocument Parse(byte[] fileBytes)
        {
            var readElement = ReadElement.None;
            var htmlBuffer = new StringBuilder();
            var openTags = new List<HtmlNode>();

            for (; _readerPosition < fileBytes.Length; _readerPosition++)
            {
                var chr = Convert.ToChar( fileBytes[_readerPosition]);
                if (readElement == ReadElement.None)
                {
                    if (IsIgnoredCharacter(chr))
                    {
                        continue;
                    }
                    if (chr == '<')
                        readElement = ReadElement.Tag;
                    else
                        readElement = ReadElement.PlainText;
                    htmlBuffer.Append(chr);
                }
                else if (readElement == ReadElement.Tag && chr == '>')
                {
                    htmlBuffer.Append(chr);
                    if (htmlBuffer.ToString().StartsWith("</"))
                    {
                        var tagName = htmlBuffer.Remove(htmlBuffer.Length - 1, 1).Remove(0, 2).ToString();
                        var closedNode = openTags.Last(t => t.TagName == tagName);
                        closedNode.IsClosed = true;
                        CheckUnclosedTags(openTags);

                        if (closedNode is HtmlDocument)
                            return closedNode as HtmlDocument;

                        readElement = ReadElement.None;
                        htmlBuffer = new StringBuilder();
                        continue;
                    }

                    var tag = htmlBuffer.ToString();
                    var node = new HtmlNode(tag);
                    node.IsClosed = HtmlCondition.EmptyTags.Any(e => e == node.TagName);
                    if (node.TagName == "html")
                        node = new HtmlDocument(tag);
                    else if (node.TagName == "head")
                        node = new HtmlHead(tag);
                    else if (node.TagName == "body")
                        node = new HtmlBody(tag);
                    else if (tag.StartsWith("<!--"))
                        node = new Comment(tag);

                    AddToParentContent(openTags, node);
                    if (!node.IsClosed)
                        openTags.Add(node);

                    readElement = ReadElement.None;
                    htmlBuffer = new StringBuilder();
                }
                else if (readElement == ReadElement.PlainText && chr == '<')
                {
                    var node = new PlainText(htmlBuffer.ToString());
                    openTags.Last().Content.Add(node);

                    readElement = ReadElement.Tag;
                    htmlBuffer = new StringBuilder("<");
                }
                else
                    htmlBuffer.Append(chr);
            }

            return openTags.FirstOrDefault(n => n is HtmlDocument) as HtmlDocument;
        }

        private static void AddToParentContent(List<HtmlNode> openTags, HtmlNode node)
        {
            if (openTags.Count == 0)
                return;

            if (!HtmlCondition.HtmlParents.ContainsKey(node.TagName))
            {
                openTags.Last().Content.Add(node);
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

        private static void CheckUnclosedTags(List<HtmlNode> openTags)
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

        private static bool IsHtmlDocument(byte[] file)
        {
            var htmlString = Encoding.UTF8.GetString(file);
            if (!htmlString.StartsWith("<!DOCTYPE"))
                return false;

            _readerPosition = htmlString.IndexOf(">") + 1;
            return true;
        }

        private static bool IsIgnoredCharacter(char chr)
        {
            if (char.IsWhiteSpace(chr) || chr == '\r' || chr == '\n')
                return true;

            return false;
        }
    }
}
