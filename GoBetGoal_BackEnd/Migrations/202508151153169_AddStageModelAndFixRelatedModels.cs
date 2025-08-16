namespace GoBetGoal_BackEnd.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStageModelAndFixRelatedModels : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TrialTemplateNotSuitForTags", "TrialTemplate_Id", "dbo.TrialTemplates");
            DropForeignKey("dbo.TrialTemplateNotSuitForTags", "NotSuitForTag_Id", "dbo.NotSuitForTags");
            DropForeignKey("dbo.SuitForTagTrialTemplates", "SuitForTag_Id", "dbo.SuitForTags");
            DropForeignKey("dbo.SuitForTagTrialTemplates", "TrialTemplate_Id", "dbo.TrialTemplates");
            DropForeignKey("dbo.TrialCautionTrialTemplates", "TrialCaution_Id", "dbo.TrialCautions");
            DropForeignKey("dbo.TrialCautionTrialTemplates", "TrialTemplate_Id", "dbo.TrialTemplates");
            DropForeignKey("dbo.TrialEffectionTrialTemplates", "TrialEffection_Id", "dbo.TrialEffections");
            DropForeignKey("dbo.TrialEffectionTrialTemplates", "TrialTemplate_Id", "dbo.TrialTemplates");
            DropForeignKey("dbo.TrialRuleTrialTemplates", "TrialRule_Id", "dbo.TrialRules");
            DropForeignKey("dbo.TrialRuleTrialTemplates", "TrialTemplate_Id", "dbo.TrialTemplates");
            DropIndex("dbo.NotSuitForTags", "IX_NotSuitForTagName");
            DropIndex("dbo.SuitForTags", "IX_SuitForTagName");
            DropIndex("dbo.TrialCautions", new[] { "CautionDescription" });
            DropIndex("dbo.TrialEffections", new[] { "EffectionDescription" });
            DropIndex("dbo.TrialRules", new[] { "RuleDescription" });
            DropIndex("dbo.TrialTemplateNotSuitForTags", new[] { "TrialTemplate_Id" });
            DropIndex("dbo.TrialTemplateNotSuitForTags", new[] { "NotSuitForTag_Id" });
            DropIndex("dbo.SuitForTagTrialTemplates", new[] { "SuitForTag_Id" });
            DropIndex("dbo.SuitForTagTrialTemplates", new[] { "TrialTemplate_Id" });
            DropIndex("dbo.TrialCautionTrialTemplates", new[] { "TrialCaution_Id" });
            DropIndex("dbo.TrialCautionTrialTemplates", new[] { "TrialTemplate_Id" });
            DropIndex("dbo.TrialEffectionTrialTemplates", new[] { "TrialEffection_Id" });
            DropIndex("dbo.TrialEffectionTrialTemplates", new[] { "TrialTemplate_Id" });
            DropIndex("dbo.TrialRuleTrialTemplates", new[] { "TrialRule_Id" });
            DropIndex("dbo.TrialRuleTrialTemplates", new[] { "TrialTemplate_Id" });
            CreateTable(
                "dbo.Stages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StageIndex = c.Int(nullable: false),
                        StageSampleImagePath = c.String(maxLength: 200),
                        StageDescription = c.String(nullable: false, maxLength: 200),
                        TrialTemplateId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TrialTemplates", t => t.TrialTemplateId, cascadeDelete: true)
                .Index(t => t.TrialTemplateId);
            
            AddColumn("dbo.TrialTemplates", "TrialSuitFor", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.TrialTemplates", "TrialNoSuitFor", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.TrialTemplates", "TrialRule", c => c.String(nullable: false, maxLength: 200));
            AddColumn("dbo.TrialTemplates", "TrialCaution", c => c.String(nullable: false, maxLength: 200));
            AddColumn("dbo.TrialTemplates", "TrialEffect", c => c.String(nullable: false, maxLength: 200));
            AddColumn("dbo.TrialTemplates", "TrialTemplatePrice", c => c.Int(nullable: false));
            AlterColumn("dbo.TrialTemplates", "TrialCategory", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.TrialTemplates", "CardColor", c => c.String(nullable: false, maxLength: 50));
            DropColumn("dbo.TrialTemplates", "TrialPrice");
            DropColumn("dbo.TrialTemplates", "IsActive");
            DropColumn("dbo.TrialTemplates", "CreatedAt");
            DropTable("dbo.NotSuitForTags");
            DropTable("dbo.SuitForTags");
            DropTable("dbo.TrialCautions");
            DropTable("dbo.TrialEffections");
            DropTable("dbo.TrialRules");
            DropTable("dbo.TrialTemplateNotSuitForTags");
            DropTable("dbo.SuitForTagTrialTemplates");
            DropTable("dbo.TrialCautionTrialTemplates");
            DropTable("dbo.TrialEffectionTrialTemplates");
            DropTable("dbo.TrialRuleTrialTemplates");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TrialRuleTrialTemplates",
                c => new
                    {
                        TrialRule_Id = c.Int(nullable: false),
                        TrialTemplate_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TrialRule_Id, t.TrialTemplate_Id });
            
            CreateTable(
                "dbo.TrialEffectionTrialTemplates",
                c => new
                    {
                        TrialEffection_Id = c.Int(nullable: false),
                        TrialTemplate_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TrialEffection_Id, t.TrialTemplate_Id });
            
            CreateTable(
                "dbo.TrialCautionTrialTemplates",
                c => new
                    {
                        TrialCaution_Id = c.Int(nullable: false),
                        TrialTemplate_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TrialCaution_Id, t.TrialTemplate_Id });
            
            CreateTable(
                "dbo.SuitForTagTrialTemplates",
                c => new
                    {
                        SuitForTag_Id = c.Int(nullable: false),
                        TrialTemplate_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SuitForTag_Id, t.TrialTemplate_Id });
            
            CreateTable(
                "dbo.TrialTemplateNotSuitForTags",
                c => new
                    {
                        TrialTemplate_Id = c.Int(nullable: false),
                        NotSuitForTag_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TrialTemplate_Id, t.NotSuitForTag_Id });
            
            CreateTable(
                "dbo.TrialRules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RuleDescription = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TrialEffections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EffectionDescription = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TrialCautions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CautionDescription = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SuitForTags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TagName = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.NotSuitForTags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TagName = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.TrialTemplates", "CreatedAt", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.TrialTemplates", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.TrialTemplates", "TrialPrice", c => c.Int(nullable: false));
            DropForeignKey("dbo.Stages", "TrialTemplateId", "dbo.TrialTemplates");
            DropIndex("dbo.Stages", new[] { "TrialTemplateId" });
            AlterColumn("dbo.TrialTemplates", "CardColor", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.TrialTemplates", "TrialCategory", c => c.Int(nullable: false));
            DropColumn("dbo.TrialTemplates", "TrialTemplatePrice");
            DropColumn("dbo.TrialTemplates", "TrialEffect");
            DropColumn("dbo.TrialTemplates", "TrialCaution");
            DropColumn("dbo.TrialTemplates", "TrialRule");
            DropColumn("dbo.TrialTemplates", "TrialNoSuitFor");
            DropColumn("dbo.TrialTemplates", "TrialSuitFor");
            DropTable("dbo.Stages");
            CreateIndex("dbo.TrialRuleTrialTemplates", "TrialTemplate_Id");
            CreateIndex("dbo.TrialRuleTrialTemplates", "TrialRule_Id");
            CreateIndex("dbo.TrialEffectionTrialTemplates", "TrialTemplate_Id");
            CreateIndex("dbo.TrialEffectionTrialTemplates", "TrialEffection_Id");
            CreateIndex("dbo.TrialCautionTrialTemplates", "TrialTemplate_Id");
            CreateIndex("dbo.TrialCautionTrialTemplates", "TrialCaution_Id");
            CreateIndex("dbo.SuitForTagTrialTemplates", "TrialTemplate_Id");
            CreateIndex("dbo.SuitForTagTrialTemplates", "SuitForTag_Id");
            CreateIndex("dbo.TrialTemplateNotSuitForTags", "NotSuitForTag_Id");
            CreateIndex("dbo.TrialTemplateNotSuitForTags", "TrialTemplate_Id");
            CreateIndex("dbo.TrialRules", "RuleDescription", unique: true);
            CreateIndex("dbo.TrialEffections", "EffectionDescription", unique: true);
            CreateIndex("dbo.TrialCautions", "CautionDescription", unique: true);
            CreateIndex("dbo.SuitForTags", "TagName", unique: true, name: "IX_SuitForTagName");
            CreateIndex("dbo.NotSuitForTags", "TagName", unique: true, name: "IX_NotSuitForTagName");
            AddForeignKey("dbo.TrialRuleTrialTemplates", "TrialTemplate_Id", "dbo.TrialTemplates", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TrialRuleTrialTemplates", "TrialRule_Id", "dbo.TrialRules", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TrialEffectionTrialTemplates", "TrialTemplate_Id", "dbo.TrialTemplates", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TrialEffectionTrialTemplates", "TrialEffection_Id", "dbo.TrialEffections", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TrialCautionTrialTemplates", "TrialTemplate_Id", "dbo.TrialTemplates", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TrialCautionTrialTemplates", "TrialCaution_Id", "dbo.TrialCautions", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SuitForTagTrialTemplates", "TrialTemplate_Id", "dbo.TrialTemplates", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SuitForTagTrialTemplates", "SuitForTag_Id", "dbo.SuitForTags", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TrialTemplateNotSuitForTags", "NotSuitForTag_Id", "dbo.NotSuitForTags", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TrialTemplateNotSuitForTags", "TrialTemplate_Id", "dbo.TrialTemplates", "Id", cascadeDelete: true);
        }
    }
}
