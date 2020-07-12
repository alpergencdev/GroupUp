namespace GroupUp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeCntInfoNonRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "ContactInfo", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "ContactInfo", c => c.String(nullable: false));
        }
    }
}
