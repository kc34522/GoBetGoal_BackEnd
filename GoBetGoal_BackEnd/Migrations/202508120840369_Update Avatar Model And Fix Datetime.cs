namespace GoBetGoal_BackEnd.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateAvatarModelAndFixDatetime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Avatars", "SortOrder", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Avatars", "SortOrder");
        }
    }
}
