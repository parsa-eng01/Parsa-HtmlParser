using HtmlParser.HtmlTags;
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
                                return head;
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
                                head.Title = htmlBuffer;
                            }
                            htmlBuffer = string.Empty;
                            readElement = ReadElement.Tag;
                        }
                        break;
                }
            }

            return head;
        }

        private HtmlBody ParseBody(string htmlString)
        {
            HtmlBody body = null;
            var readElement = ReadElement.None;
            var htmlBuffer = string.Empty;
            var openTags = new Stack<string>();

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
                                return head;
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
                                head.Title = htmlBuffer;
                            }
                            htmlBuffer = string.Empty;
                            readElement = ReadElement.Tag;
                        }
                        break;
                }
            }

            return body;
        }

        private bool IsHtmlDocument(string htmlString)
        {
            if (!htmlString.TrimStart().StartsWith("<!DOCTYPE"))
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
