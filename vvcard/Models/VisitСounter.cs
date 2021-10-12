using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vvcard.Models
{
    public class VisitСounter
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Ip { get; set; }
        public string City { get; set; }
        public string JsonResponse { get; set; }
        public string UserAgentString { get; set; }

        public int CardId { get; set; }
        public Card card { get; set; }
    }
}
