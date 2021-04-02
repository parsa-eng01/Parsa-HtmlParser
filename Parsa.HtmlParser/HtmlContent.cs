using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsa.HtmlParser
{
    public class HtmlContent : List<HtmlNode>
    {

        public HtmlNode this[string tagName] 
        {
            get => this.FirstOrDefault(n => n.TagName == tagName);
        }
    }
}
