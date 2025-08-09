namespace GoBetGoal_BackEnd.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTrialTemplateModelAndRelativeModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NotSuitForTags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TagName = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TrialTemplates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TrialTitle = c.String(nullable: false, maxLength: 100),
                        TrialDescription = c.String(nullable: false, maxLength: 200),
                        TrialFrequency = c.Int(nullable: false),
                        StageCount = c.Int(nullable: false),
                        TrialCategory = c.Int(nullable: false),
                        MaxUser = c.Int(nullable: false),
                        IsAi = c.Boolean(nullable: false),
                        TrialPrice = c.Int(nullable: false),
                        CardImagePath = c.String(nullable: false, maxLength: 200),
                        CardColor = c.String(nullable: false, maxLength: 10),
                        IsActive = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
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
                "dbo.TrialCautions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CautionDescription = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TrialEffections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EffectionDescription = c.String(nullable: false, maxLength: 150),
                        TrialEffection_Id = c.Int(),
                        TrialTemplate_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TrialEffections", t => t.TrialEffection_Id)
                .ForeignKey("dbo.TrialTemplates", t => t.TrialTemplate_Id)
                .Index(t => t.TrialEffection_Id)
                .Index(t => t.TrialTemplate_Id);
            
            CreateTable(
                "dbo.TrialRules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RuleDescription = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TrialTemplateNotSuitForTags",
                c => new
                    {
                        TrialTemplate_Id = c.Int(nullable: false),
                        NotSuitForTag_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TrialTemplate_Id, t.NotSuitForTag_Id })
                .ForeignKey("dbo.TrialTemplates", t => t.TrialTemplate_Id, cascadeDelete: true)
                .ForeignKey("dbo.NotSuitForTags", t => t.NotSuitForTag_Id, cascadeDelete: true)
                .Index(t => t.TrialTemplate_Id)
                .Index(t => t.NotSuitForTag_Id);
            
            CreateTable(
                "dbo.SuitForTagTrialTemplates",
                c => new
                    {
                        SuitForTag_Id = c.Int(nullable: false),
                        TrialTemplate_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SuitForTag_Id, t.TrialTemplate_Id })
                .ForeignKey("dbo.SuitForTags", t => t.SuitForTag_Id, cascadeDelete: true)
                .ForeignKey("dbo.TrialTemplates", t => t.TrialTemplate_Id, cascadeDelete: true)
                .Index(t => t.SuitForTag_Id)
                .Index(t => t.TrialTemplate_Id);
            
            CreateTable(
                "dbo.TrialCautionTrialTemplates",
                c => new
                    {
                        TrialCaution_Id = c.Int(nullable: false),
                        TrialTemplate_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TrialCaution_Id, t.TrialTemplate_Id })
                .ForeignKey("dbo.TrialCautions", t => t.TrialCaution_Id, cascadeDelete: true)
                .ForeignKey("dbo.TrialTemplates", t => t.TrialTemplate_Id, cascadeDelete: true)
                .Index(t => t.TrialCaution_Id)
                .Index(t => t.TrialTemplate_Id);
            
            CreateTable(
                "dbo.TrialRuleTrialTemplates",
                c => new
                    {
                        TrialRule_Id = c.Int(nullable: false),
                        TrialTemplate_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TrialRule_Id, t.TrialTemplate_Id })
                .ForeignKey("dbo.TrialRules", t => t.TrialRule_Id, cascadeDelete: true)
                .ForeignKey("dbo.TrialTemplates", t => t.TrialTemplate_Id, cascadeDelete: true)
                .Index(t => t.TrialRule_Id)
                .Index(t => t.TrialTemplate_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TrialRuleTrialTemplates", "TrialTemplate_Id", "dbo.TrialTemplates");
            DropForeignKey("dbo.TrialRuleTrialTemplates", "TrialRule_Id", "dbo.TrialRules");
            DropForeignKey("dbo.TrialEffections", "TrialTemplate_Id", "dbo.TrialTemplates");
            DropForeignKey("dbo.TrialEffections", "TrialEffection_Id", "dbo.TrialEffections");
            DropForeignKey("dbo.TrialCautionTrialTemplates", "TrialTemplate_Id", "dbo.TrialTemplates");
            DropForeignKey("dbo.TrialCautionTrialTemplates", "TrialCaution_Id", "dbo.TrialCautions");
            DropForeignKey("dbo.SuitForTagTrialTemplates", "TrialTemplate_Id", "dbo.TrialTemplates");
            DropForeignKey("dbo.SuitForTagTrialTemplates", "SuitForTag_Id", "dbo.SuitForTags");
            DropForeignKey("dbo.TrialTemplateNotSuitForTags", "NotSuitForTag_Id", "dbo.NotSuitForTags");
            DropForeignKey("dbo.TrialTemplateNotSuitForTags", "TrialTemplate_Id", "dbo.TrialTemplates");
            DropIndex("dbo.TrialRuleTrialTemplates", new[] { "TrialTemplate_Id" });
            DropIndex("dbo.TrialRuleTrialTemplates", new[] { "TrialRule_Id" });
            DropIndex("dbo.TrialCautionTrialTemplates", new[] { "TrialTemplate_Id" });
            DropIndex("dbo.TrialCautionTrialTemplates", new[] { "TrialCaution_Id" });
            DropIndex("dbo.SuitForTagTrialTemplates", new[] { "TrialTemplate_Id" });
            DropIndex("dbo.SuitForTagTrialTemplates", new[] { "SuitForTag_Id" });
            DropIndex("dbo.TrialTemplateNotSuitForTags", new[] { "NotSuitForTag_Id" });
            DropIndex("dbo.TrialTemplateNotSuitForTags", new[] { "TrialTemplate_Id" });
            DropIndex("dbo.TrialEffections", new[] { "TrialTemplate_Id" });
            DropIndex("dbo.TrialEffections", new[] { "TrialEffection_Id" });
            DropTable("dbo.TrialRuleTrialTemplates");
            DropTable("dbo.TrialCautionTrialTemplates");
            DropTable("dbo.SuitForTagTrialTemplates");
            DropTable("dbo.TrialTemplateNotSuitForTags");
            DropTable("dbo.TrialRules");
            DropTable("dbo.TrialEffections");
            DropTable("dbo.TrialCautions");
            DropTable("dbo.SuitForTags");
            DropTable("dbo.TrialTemplates");
            DropTable("dbo.NotSuitForTags");
        }
    }
}
