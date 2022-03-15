namespace PrivacyManager.Data.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using PrivacyManager.Entities;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<PrivacyManager.Data.PrivacyManagerContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "PrivacyManager.Data.PrivacyManagerContext";
        }

        protected override void Seed(PrivacyManager.Data.PrivacyManagerContext context)
        {
            //  This method will be called after migrating to the latest version.
        }

    }
}
