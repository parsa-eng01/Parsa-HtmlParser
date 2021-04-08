using System;
using System.Collections.Generic;
using System.Linq;

namespace Parsa.HtmlParser
{
    public class HtmlStyle : Dictionary<string, string>
    {
        public HtmlStyle(string style) : base(StringComparer.OrdinalIgnoreCase)
        {
            if (string.IsNullOrEmpty(style))
                return;

            style.Split(';')
                .ToList()
                .ForEach(s =>
                {
                    if (!string.IsNullOrEmpty(s))
                        base[s.Split(':')[0].Trim()] = s.Split(':')[1].Trim();
                });
        }


        public override string ToString()
            => this.Any() ? string.Join("; ", this.Select(s => $"{s.Key}:{s.Value}")) : "";
    }
}
