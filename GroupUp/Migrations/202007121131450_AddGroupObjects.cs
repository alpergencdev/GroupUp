namespace GroupUp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGroupObjects : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        GroupId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        MaxUserCapacity = c.Int(nullable: false),
                        City = c.String(nullable: false),
                        Country = c.String(nullable: false),
                        Continent = c.String(nullable: false),
                        Creator_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.GroupId)
                .ForeignKey("dbo.Users", t => t.Creator_UserId, cascadeDelete: false)
                .Index(t => t.Creator_UserId);
            
            CreateTable(
                "dbo.GroupUsers",
                c => new
                    {
                        GroupId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.GroupId, t.UserId })
                .ForeignKey("dbo.Users", t => t.GroupId, cascadeDelete: false)
                .ForeignKey("dbo.Groups", t => t.UserId, cascadeDelete: false)
                .Index(t => t.GroupId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GroupUsers", "UserId", "dbo.Groups");
            DropForeignKey("dbo.GroupUsers", "GroupId", "dbo.Users");
            DropForeignKey("dbo.Groups", "Creator_UserId", "dbo.Users");
            DropIndex("dbo.GroupUsers", new[] { "UserId" });
            DropIndex("dbo.GroupUsers", new[] { "GroupId" });
            DropIndex("dbo.Groups", new[] { "Creator_UserId" });
            DropTable("dbo.GroupUsers");
            DropTable("dbo.Groups");
        }
    }
}
