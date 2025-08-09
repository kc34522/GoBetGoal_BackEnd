namespace GoBetGoal_BackEnd.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdjustTrialTemplateRelativeModels : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.NotSuitForTags", "TagName", unique: true, name: "IX_NotSuitForTagName");
            CreateIndex("dbo.SuitForTags", "TagName", unique: true, name: "IX_SuitForTagName");
            CreateIndex("dbo.TrialCautions", "CautionDescription", unique: true);
            CreateIndex("dbo.TrialEffections", "EffectionDescription", unique: true);
            CreateIndex("dbo.TrialRules", "RuleDescription", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.TrialRules", new[] { "RuleDescription" });
            DropIndex("dbo.TrialEffections", new[] { "EffectionDescription" });
            DropIndex("dbo.TrialCautions", new[] { "CautionDescription" });
            DropIndex("dbo.SuitForTags", "IX_SuitForTagName");
            DropIndex("dbo.NotSuitForTags", "IX_NotSuitForTagName");
        }
    }
}
