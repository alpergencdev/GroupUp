namespace GroupUp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsModeratorFieldToUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "IsModerator", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "IsModerator");
        }
    }
}
