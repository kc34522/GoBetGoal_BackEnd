namespace GoBetGoal_BackEnd.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixTrialEffectionModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TrialEffections", "TrialEffection_Id", "dbo.TrialEffections");
            DropForeignKey("dbo.TrialEffections", "TrialTemplate_Id", "dbo.TrialTemplates");
            DropIndex("dbo.TrialEffections", new[] { "TrialEffection_Id" });
            DropIndex("dbo.TrialEffections", new[] { "TrialTemplate_Id" });
            CreateTable(
                "dbo.TrialEffectionTrialTemplates",
                c => new
                    {
                        TrialEffection_Id = c.Int(nullable: false),
                        TrialTemplate_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TrialEffection_Id, t.TrialTemplate_Id })
                .ForeignKey("dbo.TrialEffections", t => t.TrialEffection_Id, cascadeDelete: true)
                .ForeignKey("dbo.TrialTemplates", t => t.TrialTemplate_Id, cascadeDelete: true)
                .Index(t => t.TrialEffection_Id)
                .Index(t => t.TrialTemplate_Id);
            
            DropColumn("dbo.TrialEffections", "TrialEffection_Id");
            DropColumn("dbo.TrialEffections", "TrialTemplate_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TrialEffections", "TrialTemplate_Id", c => c.Int());
            AddColumn("dbo.TrialEffections", "TrialEffection_Id", c => c.Int());
            DropForeignKey("dbo.TrialEffectionTrialTemplates", "TrialTemplate_Id", "dbo.TrialTemplates");
            DropForeignKey("dbo.TrialEffectionTrialTemplates", "TrialEffection_Id", "dbo.TrialEffections");
            DropIndex("dbo.TrialEffectionTrialTemplates", new[] { "TrialTemplate_Id" });
            DropIndex("dbo.TrialEffectionTrialTemplates", new[] { "TrialEffection_Id" });
            DropTable("dbo.TrialEffectionTrialTemplates");
            CreateIndex("dbo.TrialEffections", "TrialTemplate_Id");
            CreateIndex("dbo.TrialEffections", "TrialEffection_Id");
            AddForeignKey("dbo.TrialEffections", "TrialTemplate_Id", "dbo.TrialTemplates", "Id");
            AddForeignKey("dbo.TrialEffections", "TrialEffection_Id", "dbo.TrialEffections", "Id");
        }
    }
}
