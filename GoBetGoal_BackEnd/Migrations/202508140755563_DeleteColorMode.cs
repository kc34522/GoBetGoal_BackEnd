namespace GoBetGoal_BackEnd.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteColorMode : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "ColorModeType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "ColorModeType", c => c.Int(nullable: false));
        }
    }
}
