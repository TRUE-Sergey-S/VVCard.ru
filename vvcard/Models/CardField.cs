using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vvcard.Models
{
    public class CardField
    {
        public int Id { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public int Order { get; set; }
        public List<ClickCardField> ClickCardFields { get; set; }

        public int CardId { get; set; }
        public Card card { get; set; }
    }
}
