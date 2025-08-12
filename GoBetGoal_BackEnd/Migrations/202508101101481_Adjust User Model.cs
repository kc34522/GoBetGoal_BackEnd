namespace GoBetGoal_BackEnd.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdjustUserModel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "PasswordHash", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "PasswordHash", c => c.String(nullable: false, maxLength: 200));
        }
    }
}
