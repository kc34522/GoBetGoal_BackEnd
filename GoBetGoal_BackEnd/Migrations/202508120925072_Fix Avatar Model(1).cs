namespace GoBetGoal_BackEnd.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixAvatarModel1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Avatars", "SortOrder", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Avatars", "SortOrder", c => c.Int());
        }
    }
}
