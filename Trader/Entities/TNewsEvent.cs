using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trader.Entities
{
    public class TNewsEvent
    {
        public string Headline { get; set; }
        public string Body { get; set; }
        public DateTime DateTime { get; set; }
    }
}
