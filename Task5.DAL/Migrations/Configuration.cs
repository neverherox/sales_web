using System.Data.Entity.Migrations;
using Task5.DAL.Context;

namespace Task5.DAL.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<SalesContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }
    }
}
