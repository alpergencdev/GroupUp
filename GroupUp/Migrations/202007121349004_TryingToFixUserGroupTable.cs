namespace GroupUp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TryingToFixUserGroupTable : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.GroupUsers", name: "GroupId", newName: "__mig_tmp__0");
            RenameColumn(table: "dbo.GroupUsers", name: "UserId", newName: "GroupId");
            RenameColumn(table: "dbo.GroupUsers", name: "__mig_tmp__0", newName: "UserId");
            RenameIndex(table: "dbo.GroupUsers", name: "IX_UserId", newName: "__mig_tmp__0");
            RenameIndex(table: "dbo.GroupUsers", name: "IX_GroupId", newName: "IX_UserId");
            RenameIndex(table: "dbo.GroupUsers", name: "__mig_tmp__0", newName: "IX_GroupId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.GroupUsers", name: "IX_GroupId", newName: "__mig_tmp__0");
            RenameIndex(table: "dbo.GroupUsers", name: "IX_UserId", newName: "IX_GroupId");
            RenameIndex(table: "dbo.GroupUsers", name: "__mig_tmp__0", newName: "IX_UserId");
            RenameColumn(table: "dbo.GroupUsers", name: "UserId", newName: "__mig_tmp__0");
            RenameColumn(table: "dbo.GroupUsers", name: "GroupId", newName: "UserId");
            RenameColumn(table: "dbo.GroupUsers", name: "__mig_tmp__0", newName: "GroupId");
        }
    }
}
