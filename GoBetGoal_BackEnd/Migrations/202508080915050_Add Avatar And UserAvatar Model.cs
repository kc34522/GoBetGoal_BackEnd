namespace GoBetGoal_BackEnd.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAvatarAndUserAvatarModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserAvatars",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Guid(nullable: false),
                        AvatarId = c.Int(nullable: false),
                        IsCurrent = c.Boolean(nullable: false),
                        AcquiredAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Avatars", t => t.AvatarId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.AvatarId);
            
            CreateTable(
                "dbo.Avatars",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AvatarImagePath = c.String(nullable: false, maxLength: 200),
                        AvatarPrice = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserAvatars", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserAvatars", "AvatarId", "dbo.Avatars");
            DropIndex("dbo.UserAvatars", new[] { "AvatarId" });
            DropIndex("dbo.UserAvatars", new[] { "UserId" });
            DropTable("dbo.Avatars");
            DropTable("dbo.UserAvatars");
        }
    }
}
