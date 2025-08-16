namespace GoBetGoal_BackEnd.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdjustUserStageImageUploadAtAllowNull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserStages", "ImageUploadAt", c => c.DateTime(precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserStages", "ImageUploadAt", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
    }
}
