namespace GoBetGoal_BackEnd.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdjustUsersTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "PasswordHash", c => c.String(nullable: false, maxLength: 200));
            AddColumn("dbo.Users", "PlayerId", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.Users", "ColorModeType", c => c.Int(nullable: false));
            AlterColumn("dbo.Users", "CreatedAt", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Users", "UpdatedAt", c => c.DateTime(precision: 7, storeType: "datetime2"));
            CreateIndex("dbo.Users", "PlayerId", unique: true);
            DropColumn("dbo.Users", "Password");
            DropColumn("dbo.Users", "ColorMode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "ColorMode", c => c.String(maxLength: 10));
            AddColumn("dbo.Users", "Password", c => c.String(nullable: false, maxLength: 200));
            DropIndex("dbo.Users", new[] { "PlayerId" });
            AlterColumn("dbo.Users", "UpdatedAt", c => c.DateTime());
            AlterColumn("dbo.Users", "CreatedAt", c => c.DateTime(nullable: false));
            DropColumn("dbo.Users", "ColorModeType");
            DropColumn("dbo.Users", "PlayerId");
            DropColumn("dbo.Users", "PasswordHash");
        }
    }
}
