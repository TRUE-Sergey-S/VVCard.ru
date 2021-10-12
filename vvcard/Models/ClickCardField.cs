using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vvcard.Models
{
    public class ClickCardField
    {
            public int Id { get; set; }
            public DateTime DateTime { get; set; }
            public string Ip { get; set; }

            public int CardFieldId { get; set; }
            public CardField cardField { get; set; }
    }
}
