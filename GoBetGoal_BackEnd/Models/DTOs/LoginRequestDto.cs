using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GoBetGoal_BackEnd.Models.DTOs
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Email 為必填欄位")]
        [EmailAddress(ErrorMessage = "請輸入有效的 Email 格式")] // 除了 Required，再加上 Email 格式驗證
        [StringLength(100, ErrorMessage = "Email 長度不可超過 100 個字元")]
        public string Email { get; set; }


        [Required(ErrorMessage = "密碼為必填欄位")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密碼長度必須介於 6 到 100 個字元之間")]
        public string Password { get; set; }
    }
}