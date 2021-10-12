using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace vvcard.Models
{
    public class tLoginModel
    {
        [Required(ErrorMessage = "Не указан E-Mail")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Не указан пароль")]
        public string Password { get; set; }
    }
}
