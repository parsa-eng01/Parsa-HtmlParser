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
            if (string.IsNullOrEmpty(style))
                return;

            _style = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            style.Split(';')
                .ToList()
                .ForEach(s => _style[s.Split('=')[0]] = s.Split('=')[1]);
        }

        private Dictionary<string, string> _style;

        public string this[string element]
        {
            get
            {
                if (_style == null)
                    return null;
                if (string.IsNullOrEmpty(element))
                    return string.Join(";", _style.Select(s => $"{s.Key}={s.Value}"));
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
    }
}
