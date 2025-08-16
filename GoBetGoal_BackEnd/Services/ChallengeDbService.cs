using GoBetGoal_BackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity; // 確保 using EntityFramework

namespace GoBetGoal_BackEnd.Services
{
    public class ChallengeDbService
    {
        private readonly Context _db = new Context();

        /// <summary>
        /// 根據試煉ID和關卡索引，非同步地從資料庫獲取關卡的完整資訊
        /// </summary>
        /// <param name="trialId">試煉 ID</param>
        /// <param name="stageIndex">關卡索引</param>
        /// <returns>包含關卡與試煉總規則的 Stage 物件</returns>
        public async Task<Stage> GetStageWithTrialAsync(int trialId, int stageIndex)
        {
            // 使用 .Include() 來進行關聯查詢，這樣我們在拿到 Stage 的同時，
            // 也一併把關聯的 TrialTemplate (包含了總規則 TrialRule) 的資料撈出來。
            return await _db.Stages
                .Include(s => s.TrialTemplate)
                .FirstOrDefaultAsync(s => s.TrialTemplateId == trialId && s.StageIndex == stageIndex);
        }
    }
}