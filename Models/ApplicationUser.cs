using Microsoft.AspNet.Identity.EntityFramework;

namespace YobitTradingBot.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Вносим в дополнение к стандартным параметры для пользователя ниже
        public string SecretAPI  { get; set; }
        public string PrivatAPI { get; set; }
        public ApplicationUser()
        {
        }
    }
}