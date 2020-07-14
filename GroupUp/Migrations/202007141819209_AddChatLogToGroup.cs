namespace GroupUp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddChatLogToGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Groups", "ChatLog", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Groups", "ChatLog");
        }
    }
}
