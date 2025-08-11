namespace GoBetGoal_BackEnd.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveOldUserNickNameIndex : DbMigration
    {
        public override void Up()
        {
            // 僅移除舊的 IX_UserNickName 索引
            DropIndex("dbo.Users", "IX_UserNickName");
        }
        
        public override void Down()
        {
            // 恢復舊的 IX_UserNickName 索引
            CreateIndex("dbo.Users", "NickName", unique: true, name: "IX_UserNickName");
        }
    }
}
