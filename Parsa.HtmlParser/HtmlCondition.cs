using System.Collections.Generic;

namespace Parsa.HtmlParser
{
    public class HtmlCondition
    {
        public static readonly string[] EmptyTags =
        {
            "area",
            "base",
            "br",
            "col",
            "embed",
            "hr",
            "img",
            "input",
            "keygen",
            "link",
            "meta",
            "param",
            "source",
            "track",
            "wbr"
        };


        private static Dictionary<string, string[]> _htmlParents;
        public static Dictionary<string, string[]> HtmlParents = _htmlParents ?? (_htmlParents = new Dictionary<string, string[]>
        {
            {"tr", new string[]{ "table", "tbody", "thead", "tfoot" } },
            {"td", new string[]{ "th", "tr" } },
            {"tbody", new string[]{ "table"} },
            {"thead", new string[]{ "table"} },
            {"tfoot", new string[]{ "table"} },
            {"il", new string[]{ "ol", "ul", "menu" } },
        });
    }
}
