using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace vvcard.Models
{
    public class Card
    {
        public int Id { get;set; }
        
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 20 символов")]
        [RegularExpression(@"[A-Za-z][A-Za-z0-9_$]{0,20}", ErrorMessage = "Значение не должно начинаться с цифры, можно использовать только буквы латинского алфавита, цифры и нижнее подчеркивание \"_\"")]
        [Remote(action: "CheckPublicID", controller: "Card", ErrorMessage = "Такой адрес уже зарезервирован")]
        public string PublicID { get; set; }
        
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 50 символов")]
        public string Name { get; set; }
        
        public string CssToString { get; set; }
        public bool IsPrivate { get; set; }
        public List<CardField> cardFields { get; set; }
        public List<VisitСounter> visitСounter { get; set; }

        public string UserId { get; set; }
        public User user { get; set; }
    }
}
