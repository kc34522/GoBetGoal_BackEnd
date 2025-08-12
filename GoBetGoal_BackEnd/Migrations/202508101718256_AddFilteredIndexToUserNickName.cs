namespace GoBetGoal_BackEnd.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFilteredIndexToUserNickName : DbMigration
    {
        public override void Up()
        {
            //DropIndex("dbo.Users", "IX_UserPlayerId");
            
            // 使用 Sql() 方法來執行原生 T-SQL 指令，建立篩選過的唯一索引
            Sql("CREATE UNIQUE NONCLUSTERED INDEX IX_UserNickName_Filtered ON dbo.Users(NickName) WHERE NickName IS NOT NULL");

        }

        public override void Down()
        {
            // 對應的 Down() 操作是刪除這個索引
            // 這裡可以直接使用 DropIndex，因為它只需要知道索引的名稱
            DropIndex("dbo.Users", "IX_UserNickName_Filtered");
        }
    }
}
