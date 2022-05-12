namespace PrivacyManager.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AttemptedQuestions", "UploadFileName", c => c.String());
            AddColumn("dbo.AttemptedQuestions", "UploadFilePath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AttemptedQuestions", "UploadFilePath");
            DropColumn("dbo.AttemptedQuestions", "UploadFileName");
        }
    }
}
