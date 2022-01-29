using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace vvcard.Models
{
    public class CardField
    {
        public int Id { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public int Order { get; set; }
        public int? FieldType { get; set; }
        public List<ClickCardField> ClickCardFields { get; set; }

        public int CardId { get; set; }
        public Card Card { get; set; }
        
        public enum FieldTypeEnum
        {
            [Display(Name = @"Текст")]
            Text = 1,
            [Display(Name = @"Ссылка")]
            Href,
            [Display(Name = @"Телефон")]
            Phone,
            [Display(Name = @"E-Mail")]
            Email
        };
    }
}
