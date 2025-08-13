using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoBetGoal_BackEnd.Models.DTOs
{
    /// <summary>
    /// 通用的結構化錯誤回應
    /// </summary>
    public class ErrorResponseDto
    {
        /// <summary>
        /// 機器可讀的錯誤代碼
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// 人類可讀的錯誤訊息
        /// </summary>
        public string Message { get; set; }
    }
}