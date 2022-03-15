using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PrivacyManager.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;

namespace PrivacyManager.Data
{
    public class PrivacyManagerDatabaseInitializer : CreateDatabaseIfNotExists<PrivacyManagerContext>
    {
        protected override void Seed(PrivacyManagerContext context)
        {
            //add these roles to database if does not exists
            SeedRoles(context);

            //add these users to database if not exists
            SeedUsers(context);
        }
        
        public void SeedRoles(PrivacyManager.Data.PrivacyManagerContext context)
        {
            List<IdentityRole> rolesInPrivacyManager = new List<IdentityRole>();

            rolesInPrivacyManager.Add(new IdentityRole() { Name = "Administrator" });
            rolesInPrivacyManager.Add(new IdentityRole() { Name = "User" });

            var rolesStore = new RoleStore<IdentityRole>(context);
            var rolesManager = new RoleManager<IdentityRole>(rolesStore);
            
            foreach (IdentityRole role in rolesInPrivacyManager)
            {
                if (!rolesManager.RoleExists(role.Name))
                {
                    var result = rolesManager.Create(role);

                    if (result.Succeeded) continue;
                }
            }
        }

        public void SeedUsers(PrivacyManager.Data.PrivacyManagerContext context)
        {
            var usersStore = new UserStore<PrivacyManagerUser>(context);
            var usersManager = new UserManager<PrivacyManagerUser>(usersStore);

            PrivacyManagerUser admin = new PrivacyManagerUser();
            admin.Email = "admin@email.com";
            admin.UserName = "admin";
            var password = "admin123";

            if (usersManager.FindByEmail(admin.Email) == null)
            {
                var result = usersManager.Create(admin, password);

                if(result.Succeeded)
                {
                    //add necessary roles to admin
                    usersManager.AddToRole(admin.Id, "Administrator");
                    usersManager.AddToRole(admin.Id, "User");
                }
            }
        }
    }
}