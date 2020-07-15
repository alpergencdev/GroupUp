namespace GroupUp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddVerificationProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "VerificationCode", c => c.Int());
            AddColumn("dbo.Users", "IsVerified", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "IsVerified");
            DropColumn("dbo.Users", "VerificationCode");
        }
    }
}
