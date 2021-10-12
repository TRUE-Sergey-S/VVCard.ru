using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace vvcard.Models
{
    public class User : IdentityUser
    {
        public override string Id { get; set; }
        public override string UserName { get; set; }
        public List<Card> Cards = new List<Card>();
    }
}
