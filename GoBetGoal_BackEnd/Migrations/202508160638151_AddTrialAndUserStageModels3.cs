namespace GoBetGoal_BackEnd.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTrialAndUserStageModels3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserAvatars", "AvatarId", "dbo.Avatars");
            DropForeignKey("dbo.UserAvatars", "UserId", "dbo.Users");
            DropForeignKey("dbo.Stages", "TrialTemplateId", "dbo.TrialTemplates");
            CreateTable(
                "dbo.Trials",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Guid(nullable: false),
                        TrialTemplateId = c.Int(nullable: false),
                        StartTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        EndTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        TrialDeposit = c.Int(nullable: false),
                        TrialName = c.String(nullable: false, maxLength: 100),
                        TrialStatus = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TrialTemplates", t => t.TrialTemplateId, cascadeDelete: false)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: false)
                .Index(t => t.UserId)
                .Index(t => t.TrialTemplateId);
            
            CreateTable(
                "dbo.UserStages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Guid(nullable: false),
                        TrialId = c.Int(nullable: false),
                        StageId = c.Int(nullable: false),
                        UploadImagePath = c.String(maxLength: 500),
                        ImageUploadAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ChanceRemain = c.Int(nullable: false),
                        StartTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        EndTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsCheat = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stages", t => t.StageId, cascadeDelete: false)
                .ForeignKey("dbo.Trials", t => t.TrialId, cascadeDelete: false)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: false)
                .Index(t => t.UserId)
                .Index(t => t.TrialId)
                .Index(t => t.StageId);
            
            AddForeignKey("dbo.UserAvatars", "AvatarId", "dbo.Avatars", "Id", cascadeDelete: false);
            AddForeignKey("dbo.UserAvatars", "UserId", "dbo.Users", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Stages", "TrialTemplateId", "dbo.TrialTemplates", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Stages", "TrialTemplateId", "dbo.TrialTemplates");
            DropForeignKey("dbo.UserAvatars", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserAvatars", "AvatarId", "dbo.Avatars");
            DropForeignKey("dbo.Trials", "UserId", "dbo.Users");
            DropForeignKey("dbo.Trials", "TrialTemplateId", "dbo.TrialTemplates");
            DropForeignKey("dbo.UserStages", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserStages", "TrialId", "dbo.Trials");
            DropForeignKey("dbo.UserStages", "StageId", "dbo.Stages");
            DropIndex("dbo.UserStages", new[] { "StageId" });
            DropIndex("dbo.UserStages", new[] { "TrialId" });
            DropIndex("dbo.UserStages", new[] { "UserId" });
            DropIndex("dbo.Trials", new[] { "TrialTemplateId" });
            DropIndex("dbo.Trials", new[] { "UserId" });
            DropTable("dbo.UserStages");
            DropTable("dbo.Trials");
            AddForeignKey("dbo.Stages", "TrialTemplateId", "dbo.TrialTemplates", "Id", cascadeDelete: false);
            AddForeignKey("dbo.UserAvatars", "UserId", "dbo.Users", "Id", cascadeDelete: false);
            AddForeignKey("dbo.UserAvatars", "AvatarId", "dbo.Avatars", "Id", cascadeDelete: false);
        }
    }
}
