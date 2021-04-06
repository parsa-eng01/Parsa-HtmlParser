using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsa.HtmlParser
{
    public enum QouteModes
    {
        DoubleQoute,
        SingleQoute,
        NoQoute
    }

    public class HtmlAttributes : Dictionary<string, string>
    {
        public HtmlAttributes(string attributeStr) : base(StringComparer.OrdinalIgnoreCase)
        {
            if (string.IsNullOrEmpty(attributeStr))
                return;

            foreach (var attr in GetAttributes(attributeStr))
                Add(attr.Key, attr.Value);
        }

        public HtmlAttributes(Dictionary<string, string> attributes) : base(attributes)
        {

        }

        private Dictionary<string, string> GetAttributes(string attributeStr)
        {
            if (!attributeStr.Contains("="))
                return new Dictionary<string, string>();

            var name = string.Empty;
            var reader = string.Empty;
            var attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var chr in attributeStr)
            {
                reader += chr.ToString();
                if (chr == '=')
                {
                    reader = reader.Remove(reader.Length - 1);
                    var value = string.Empty;
                    if (reader.Contains(" "))
                    {
                        value = reader.Substring(0, reader.LastIndexOf(' ')).Trim();
                        if (value.StartsWith("'") || value.StartsWith("\""))
                        {
                            value = value.Substring(0, reader.LastIndexOf(value[0]) + 1);
                            var temp = reader.Replace(value, "").Trim();
                            if (temp.Contains(" "))
                            {
                                for (int i = 0; i < temp.Split(' ').Length - 1; i++)
                                    attributes[temp.Split(' ')[i]] = string.Empty;
                            }
                            value = value.Remove(0, 1).Remove(value.Length - 2);
                        }

                        if (!string.IsNullOrEmpty(name))
                            attributes[name] = value;
                        name = reader.Substring(reader.LastIndexOf(' ')).Trim();
                    }
                    else 
                    {
                        if (!string.IsNullOrEmpty(name))
                            attributes[name] = value;
                        name = reader;
                    }
                    reader = string.Empty;
                }
            }
            reader = reader.Trim();
            if (reader.StartsWith("'") || reader.StartsWith("\""))
                reader = reader.Remove(0, 1).Remove(reader.Length - 2);
            attributes[name] = reader;

            return attributes;
        }




        public override string ToString()
            => string.Join(" ", this.Select(a => $"{a.Key}=\"{a.Value}\""));
    }
}
