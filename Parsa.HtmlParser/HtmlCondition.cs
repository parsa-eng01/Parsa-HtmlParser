using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static readonly IReadOnlyList<IReadOnlyList<string>> ParentChildTags = new string[][]
        {
            new string[] {"table","tr","td" },
        };
    }
}
