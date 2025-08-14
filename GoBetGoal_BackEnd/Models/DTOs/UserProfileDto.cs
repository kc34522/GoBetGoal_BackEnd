using GoBetGoal_BackEnd.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoBetGoal_BackEnd.Models.DTOs
{
    /// <summary>
    /// 使用者的個人設定檔
    /// </summary>
    public class UserProfileDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string PlayerId { get; set; }
        public string NickName { get; set; }
        public int BagelCount { get; set; }
        public int CheatBlanketCount { get; set; }
        public ColorModeType ColorMode { get; set; }
        public DateTime CreatedAt { get; set; }

        // 我們可以額外加入一個欄位，方便前端直接顯示使用者「目前」的頭像
        public int? CurrentAvatarId { get; set; }
        public string CurrentAvatarUrl { get; set; }
    }
}