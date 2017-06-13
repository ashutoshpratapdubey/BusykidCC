using System.Data.Entity.Migrations;

namespace LeapSpring.MJC.Data
{
    public sealed class Configuration : DbMigrationsConfiguration<MJCDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }
    }
}
