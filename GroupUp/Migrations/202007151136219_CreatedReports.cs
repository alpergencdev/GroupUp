namespace GroupUp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatedReports : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GroupReports",
                c => new
                    {
                        GroupReportId = c.Int(nullable: false, identity: true),
                        Reason = c.String(nullable: false),
                        DetailedDescription = c.String(nullable: false),
                        TargetGroup_GroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.GroupReportId)
                .ForeignKey("dbo.Groups", t => t.TargetGroup_GroupId, cascadeDelete: false)
                .Index(t => t.TargetGroup_GroupId);
            
            CreateTable(
                "dbo.UserReports",
                c => new
                    {
                        UserReportId = c.Int(nullable: false, identity: true),
                        Reason = c.String(nullable: false),
                        DetailedDescription = c.String(nullable: false),
                        TargetUser_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserReportId)
                .ForeignKey("dbo.Users", t => t.TargetUser_UserId, cascadeDelete: false)
                .Index(t => t.TargetUser_UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserReports", "TargetUser_UserId", "dbo.Users");
            DropForeignKey("dbo.GroupReports", "TargetGroup_GroupId", "dbo.Groups");
            DropIndex("dbo.UserReports", new[] { "TargetUser_UserId" });
            DropIndex("dbo.GroupReports", new[] { "TargetGroup_GroupId" });
            DropTable("dbo.UserReports");
            DropTable("dbo.GroupReports");
        }
    }
}
