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
        [MaxLength(100)]
        [Display(Name = "帳號（Email）")]
        public string UserName { get; set; }

        // 使用者 ID（可作為代號/識別）
        //[Required(ErrorMessage = "{0} 為必填")]
        //[MaxLength(100)]
        //[Display(Name = "使用者ID")]
        //public string UserId { get; set; }

        // 密碼（建議實際部署前應加密）
        [Required(ErrorMessage = "{0} 為必填")]
        [MaxLength(200)]
        //[DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; }

        [Required(ErrorMessage = "{0} 為必填")]
        [MaxLength(50)]
        [Display(Name = "暱稱")]
        public string NickName { get; set; }

        [MaxLength(10)]
        public string ColorMode { get; set; }

        public int BagelCount { get; set; } = 0;

        public int CheatBlanketCount { get; set; } = 0;

        [Display(Name = "建立時間")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "更新時間")]
        public DateTime? UpdatedAt { get; set; }
    }
}