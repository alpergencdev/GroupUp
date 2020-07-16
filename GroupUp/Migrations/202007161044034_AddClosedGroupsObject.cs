namespace GroupUp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddClosedGroupsObject : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClosedGroups",
                c => new
                    {
                        ClosedGroupId = c.Int(nullable: false, identity: true),
                        Group_GroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ClosedGroupId)
                .ForeignKey("dbo.Groups", t => t.Group_GroupId, cascadeDelete: false)
                .Index(t => t.Group_GroupId);
            
            CreateTable(
                "dbo.ClosedGroupUsers",
                c => new
                    {
                        ClosedGroup_ClosedGroupId = c.Int(nullable: false),
                        User_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ClosedGroup_ClosedGroupId, t.User_UserId })
                .ForeignKey("dbo.ClosedGroups", t => t.ClosedGroup_ClosedGroupId, cascadeDelete: false)
                .ForeignKey("dbo.Users", t => t.User_UserId, cascadeDelete: false)
                .Index(t => t.ClosedGroup_ClosedGroupId)
                .Index(t => t.User_UserId);
            
            AddColumn("dbo.Groups", "IsClosed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ClosedGroupUsers", "User_UserId", "dbo.Users");
            DropForeignKey("dbo.ClosedGroupUsers", "ClosedGroup_ClosedGroupId", "dbo.ClosedGroups");
            DropForeignKey("dbo.ClosedGroups", "Group_GroupId", "dbo.Groups");
            DropIndex("dbo.ClosedGroupUsers", new[] { "User_UserId" });
            DropIndex("dbo.ClosedGroupUsers", new[] { "ClosedGroup_ClosedGroupId" });
            DropIndex("dbo.ClosedGroups", new[] { "Group_GroupId" });
            DropColumn("dbo.Groups", "IsClosed");
            DropTable("dbo.ClosedGroupUsers");
            DropTable("dbo.ClosedGroups");
        }
    }
}
