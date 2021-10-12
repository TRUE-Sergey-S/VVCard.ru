using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace vvcard.Models
{
    public class tRegisterModel
    {
        [Display(Name = "Логин")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 50 символов")]
        //[RegularExpression(@"[^a-zA-Z0-9]", ErrorMessage = "Можно использовать только буквы латинского алфавита и цифры")]
        [Remote(action: "CheckUserName", controller: "Account", ErrorMessage = "Имя пользователя уже используется")]
        [Required(ErrorMessage = "Не указано имя пользователя")]
        public string UserName { get; set; }

        [Display(Name = "Е-Mail")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес")]
        [Remote(action: "CheckEmail", controller: "tAccount", ErrorMessage = "Email уже используется")]
        [Required(ErrorMessage = "Не указан E-Mail")]
        public string Email { get; set; }

        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Длина строки должна быть от 6 до 50 символов")]
        [Required(ErrorMessage = "Не указан пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль не совпадает")]
        public string ConfirmPassword { get; set; }
    }
}
