using GroupUp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GroupUp.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<GroupUp.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(GroupUp.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
            var roleManager =new RoleManager<IdentityRole>(roleStore); 
            roleManager.Create(new IdentityRole("SecurityLevel0"));
            roleManager.Create(new IdentityRole("SecurityLevel1"));
            roleManager.Create(new IdentityRole("SecurityLevel2"));
            roleManager.Create(new IdentityRole("Moderator"));
        }
    }
}
