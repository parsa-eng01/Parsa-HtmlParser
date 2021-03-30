using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlParser
{
    public class HtmlStyle
    {
        public HtmlStyle(string style)
        {
            _style = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrEmpty(style))
                return;

            style.Split(';')
                .ToList()
                .ForEach(s =>
                {
                    if (!string.IsNullOrEmpty(s))
                        _style[s.Split(':')[0].Trim()] = s.Split(':')[1].Trim();
                });
        }

        private Dictionary<string, string> _style;

        public string this[string element]
        {
            get
            {
                if (_style == null)
                    return null;
                if (string.IsNullOrEmpty(element))
                    return null;
                if (!_style.ContainsKey(element))
                    return null;

                return _style[element];
            }
            set
            {
                if (_style == null)
                    _style = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                _style[element] = value;
            }
        }



        public override string ToString()
            => _style.Any() ? string.Join("; ", _style.Select(s => $"{s.Key}:{s.Value}")) : "";
    }
}
