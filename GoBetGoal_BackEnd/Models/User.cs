﻿using GoBetGoal_BackEnd.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GoBetGoal_BackEnd.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // 登入用 Email
        [Required(ErrorMessage = "{0} 為必填")]
        [Index("IX_UserEmail", IsUnique = true)]
        [EmailAddress(ErrorMessage = "請輸入有效的 Email 格式")] // 除了 Required，再加上 Email 格式驗證
        [StringLength(100, ErrorMessage = "Email 長度不可超過 100 個字元")]
        [Display(Name = "帳號（Email）")]
        public string Email { get; set; }

        // 密碼（建議實際部署前應加密）
        [Required(ErrorMessage = "{0} 為必填")]
        [StringLength(100)] // BCrypt 產生的 Hash 長度固定為 60，但設定長一點以保持彈性
        //[DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string PasswordHash { get; set; }

        // 使用者 ID（可作為代號/識別）
        //[Index("IX_UserPlayerId", IsUnique = true)]
        [Required(ErrorMessage = "{0} 為必填")]
        [StringLength(100)]
        [Display(Name = "使用者ID")]
        public string PlayerId { get; set; }

        //[Required(ErrorMessage = "{0} 為必填")]
        [Index("IX_UserNickName", IsUnique = true)]
        [StringLength(50)]
        [Display(Name = "暱稱")]
        public string NickName { get; set; }

        public ColorModeType ColorModeType { get; set; } = ColorModeType.Dark; // 預設DarkMode

        public int BagelCount { get; set; } = 0;

        public int CheatBlanketCount { get; set; } = 0;

        [Column(TypeName = "datetime2")]
        [Display(Name = "建立時間")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        [Column(TypeName = "datetime2")]
        [Display(Name = "更新時間")]
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<UserAvatar> UserAvatars { get; set; }
    }
}