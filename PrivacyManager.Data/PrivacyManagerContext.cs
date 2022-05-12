using Microsoft.AspNet.Identity.EntityFramework;
using PrivacyManager.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrivacyManager.Data
{
    public class PrivacyManagerContext : IdentityDbContext<PrivacyManagerUser>, IDisposable
    {
        public PrivacyManagerContext()
            : base("PrivacyManagerConnection", throwIfV1Schema: false)
        {
            //Database.SetInitializer<PrivacyManagerContext>(new CreateDatabaseIfNotExists<PrivacyManagerContext>());

            Database.SetInitializer<PrivacyManagerContext>(new PrivacyManagerDatabaseInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // This needs to go before the other rules!

            modelBuilder.Entity<PrivacyManagerUser>().ToTable("Users");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins");
        }

        public static PrivacyManagerContext Create()
        {
            return new PrivacyManagerContext();
        }

        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<StudentQuiz> StudentQuizzes { get; set; }
        public DbSet<AttemptedQuestion> AttemptedQuestions { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<UploadFile> UploadFiles { get; set; }
        public DbSet<AttemptedOptionFile> AttemptedOptionFiles { get; set; }
    }
}
