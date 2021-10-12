using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vvcard
{
    public static class ServiceAndBotDetector
    {
        public static bool Check(string userAgent) {
            List<string> userAgentBotAndService = new List<string> { "" };
            
            foreach (var item in userAgentBotAndService)
            {
                if (item == userAgent)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
