using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace YobitTradingBot.Models
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext() : base("IdentityDb")
        {
            // Сюда добавим нициализацию БД
            Database.SetInitializer<ApplicationContext>(new MigrateDatabaseToLatestVersion<ApplicationContext, ConfigurationDB>());

        }

        public DbSet<BotModel> BotModels { get; set; }

        public static ApplicationContext Create()
        {
            return new ApplicationContext();
        }
    }

    public class ConfigurationDB : DbMigrationsConfiguration<ApplicationContext>
    {
        public ConfigurationDB()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }
        protected override void Seed(ApplicationContext context)
        {
            // Наполнение БД по умолчанию
            base.Seed(context);
        }
    }
}