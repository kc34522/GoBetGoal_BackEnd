namespace GoBetGoal_BackEnd.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddVerificationModeInStageModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stages", "VerificationMode", c => c.String(maxLength: 50));
            AlterColumn("dbo.Stages", "StageSampleImagePath", c => c.String(maxLength: 500));
            AlterColumn("dbo.Stages", "StageDescription", c => c.String(nullable: false, maxLength: 300));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Stages", "StageDescription", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.Stages", "StageSampleImagePath", c => c.String(maxLength: 200));
            DropColumn("dbo.Stages", "VerificationMode");
        }
    }
}
