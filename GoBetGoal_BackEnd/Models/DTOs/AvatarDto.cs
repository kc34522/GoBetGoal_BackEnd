using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GoBetGoal_BackEnd.Models.DTOs
{
    /// <summary>
    /// 頭像的資料物件
    /// </summary>
    public class AvatarDto
    {
        public int AvatarId { get; set; }
        public int SortOrder { get; set; }
        public string AvatarImagePath { get; set; }
        public int AvatarPrice { get; set; }
        public bool IsLocked { get; set; }
        public bool IsUnlocked { get; set; }

    }
}