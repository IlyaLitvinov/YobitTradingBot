using System.ComponentModel.DataAnnotations;

namespace YobitTradingBot.Models
{
    public class EditModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string SecretAPI { get; set; }
        public string PrivatAPI { get; set; }
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
    }
}