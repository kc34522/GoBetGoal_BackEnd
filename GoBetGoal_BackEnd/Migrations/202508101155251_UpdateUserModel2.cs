namespace GoBetGoal_BackEnd.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateUserModel2 : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.Users", name: "IX_PlayerId", newName: "IX_UserPlayerId");
            AddColumn("dbo.Users", "Email", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Users", "PasswordHash", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Users", "NickName", c => c.String(maxLength: 50));
            CreateIndex("dbo.Users", "Email", unique: true, name: "IX_UserEmail");
            CreateIndex("dbo.Users", "NickName", unique: true, name: "IX_UserNickName");
            DropColumn("dbo.Users", "UserName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "UserName", c => c.String(nullable: false, maxLength: 100));
            DropIndex("dbo.Users", "IX_UserNickName");
            DropIndex("dbo.Users", "IX_UserEmail");
            AlterColumn("dbo.Users", "NickName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Users", "PasswordHash", c => c.String(nullable: false, maxLength: 200));
            DropColumn("dbo.Users", "Email");
            RenameIndex(table: "dbo.Users", name: "IX_UserPlayerId", newName: "IX_PlayerId");
        }
    }
}
