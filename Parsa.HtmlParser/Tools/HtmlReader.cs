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
            string htmlString = File.ReadAllText(file, Encoding.UTF8);

            if (string.IsNullOrEmpty(htmlString))
                return null;
            _readerPosition = 0;
            if (!IsHtmlDocument(htmlString))
                return null;

            var doc = Parse(htmlString);

            return doc;
        }

        public static HtmlDocument Parse(string htmlString)
        {
            var readElement = ReadElement.None;
            var htmlBuffer = string.Empty;
            var index = _readerPosition;
            var openTags = new List<HtmlNode>();

            for (; _readerPosition < htmlString.Length; _readerPosition++)
            {
                var chr = htmlString[_readerPosition];
                
                try
                {
                    if (readElement == ReadElement.None)
                    {
                        if (IsIgnoredCharacter(chr))
                        {
                            //index++;
                            continue;
                        }
                        if (chr == '<')
                            readElement = ReadElement.Tag;
                        else
                            readElement = ReadElement.PlainText;
                        htmlBuffer += chr;
                    }
                    else if (readElement == ReadElement.Tag && chr == '>')
                    {
                        //htmlBuffer = "<" + htmlString.Substring(index + 1, _readerPosition - index);
                        //for (int i = index + 1; i <= _readerPosition; i++)
                        //{
                        //    if (htmlBuffer.Length == 1 && char.IsWhiteSpace(chr))
                        //        continue;
                        //    htmlBuffer += htmlString[i];
                        //}

                        htmlBuffer += chr;
                        if (htmlBuffer.StartsWith("</"))
                        {
                            var tagName = htmlBuffer.Remove(htmlBuffer.Length - 1).Remove(0, 2);
                            var closedNode = openTags.Last(t => t.TagName == tagName);
                            closedNode.IsClosed = true;
                            CheckUnclosedTags(openTags);

                            if (closedNode is HtmlDocument)
                                return closedNode as HtmlDocument;

                            //index = _readerPosition + 1;
                            readElement = ReadElement.None;
                            htmlBuffer = string.Empty;
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


                        if (_readerPosition % 10000 == 0)
                        {
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                        }

                            //index = _readerPosition + 1;
                            readElement = ReadElement.None;
                        htmlBuffer = string.Empty;
                    }
                    else if (readElement == ReadElement.PlainText && chr == '<')
                    {
                        //htmlBuffer = htmlString.Substring(index, _readerPosition - index).Trim();
                        //htmlBuffer = string.Empty;
                        //for (int i = index; i < _readerPosition; i++)
                        //    htmlBuffer += htmlString[i];
                        //htmlBuffer += chr;
                        var node = new PlainText(htmlBuffer);
                        openTags.Last().Content.Add(node);

                        readElement = ReadElement.Tag;
                        //index = _readerPosition;
                        htmlBuffer = "<";
                    }
                    else
                    {
                        htmlBuffer += chr;
                    }

                }
                catch (Exception error)
                {
                    throw new Exception($"open tags count {openTags.Count}, html buffer {htmlBuffer}," +
                        $" reader position {_readerPosition}, index {index}, reader element {readElement}", error);
                }
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

        private static bool IsHtmlDocument(string htmlString)
        {
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
