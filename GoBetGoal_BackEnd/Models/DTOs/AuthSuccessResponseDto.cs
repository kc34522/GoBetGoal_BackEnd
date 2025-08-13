using System;

namespace GoBetGoal_BackEnd.Models.DTOs
{
    /// <summary>
    /// 註冊/登入成功的回應
    /// </summary>
    public class AuthSuccessResponseDto
    {
        public string Message { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public long ExpiresIn { get; set; }
    }
}