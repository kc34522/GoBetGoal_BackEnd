using GoBetGoal_BackEnd.Models;
using GoBetGoal_BackEnd.Models.DTOs;
using GoBetGoal_BackEnd.Security;
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace GoBetGoal_BackEnd.Controllers
{
    public class AuthController : ApiController
    {
        // 準備資料庫連線
        private readonly Context _db = new Context();

        [HttpPost]
        [Route("api/auth/register")]
        [AllowAnonymous] // 標示此 API 不需要登入(不需要 JWT)即可存取
        public IHttpActionResult RegisterStepOne(RegisterStepOneRequestDto model)
        {
            // 驗證傳入的資料是否符合 DTO 的規則 (例如 [Required], [EmailAddress])
            if (!ModelState.IsValid)
            {
                // 如果驗證失敗，回傳 400 Bad Request 並附上錯誤訊息
                return BadRequest(ModelState);
            }

            // ---新增：將 Email 正規化為小寫-- -
            // ToLowerInvariant() 是用於處理非顯示用的、程式內部比較的標準方法
            string normalizedEmail = model.Email.ToLowerInvariant();

            // 檢查 Email 是否已經被註冊
            bool IsEmailTaken = _db.Users.Any(u => u.Email == normalizedEmail);
            if (IsEmailTaken)
            {
                var errorResponse = new
                {
                    errorCode = "EMAIL_ALREADY_EXISTS",
                    message = "此 Email 已被註冊，請嘗試使用其他 Email 或登入現有帳號。" // 是否直接改成網頁跳出來的紅色警告訊息?
                };

                // 如果 Email 已存在，回傳 409 Conflict，並附上易於理解的錯誤訊息
                return Content(HttpStatusCode.Conflict, errorResponse);
            }

            // 後端自動生成一個獨特的 PlayerId
            string newPlayerId = Guid.NewGuid().ToString(); // 測試用 待修改 或寫進MODEL裡(如果不檢查的話)

            // 建立一個新的 User 物件，準備存入資料庫
            var newUser = new User
            {
                Email = normalizedEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                PlayerId = newPlayerId
            };

            _db.Users.Add(newUser);

            // 找出所有免費的頭像
            var freeAvatars = _db.Avatars.Where(u => u.AvatarPrice == 0 && u.IsActive).ToList();

            // 為每一個免費頭像，建立關聯紀錄
            foreach (var avatar in freeAvatars)
            {
                var newUserAvatar = new UserAvatar
                {
                    UserId = newUser.Id,
                    AvatarId = avatar.Id,
                    AcquiredAt = DateTime.UtcNow
                };
                _db.UserAvatars.Add(newUserAvatar);
            }

            try
            {
                // 4. 將所有變更 (新增 User 和 新增多筆 UserAvatar) 在一個交易中，一次性存入資料庫
                _db.SaveChanges();
            }
            // 捕捉 Entity Framework 在儲存前的模型驗證錯誤
            //catch (DbEntityValidationException ex)
            //{
            //    // 這可以捕捉到更詳細的 Model 層驗證錯誤，對於偵錯非常有幫助
            //    // (這個錯誤會被下面的 GlobalExceptionFilter 捕捉到並記錄)
            //    return InternalServerError(ex);
            //}
            // 捕捉所有其他可能的例外狀況
            catch (Exception ex)
            {
                // 這裡的錯誤會被您的 GlobalExceptionFilter 捕捉到
                // 所以 InternalServerError() 主要是為了中斷執行
                // 您也可以在這裡手動記錄 Log
                return InternalServerError(ex);
            }

         

            // 註冊成功後，立即產生 JWT，讓前端可以無縫進行下一步
            var jwtAuthUtil = new JwtAuthUtility();
            // 呼叫您寫好的工具來產生 Token
            string token = jwtAuthUtil.GenerateToken(newUser);

            // 3 個月大約的總秒數
            long totalSeconds = (long)TimeSpan.FromDays(90).TotalSeconds;

            return Ok(new
            {
                message = "帳號建立成功，請繼續完善資料",
                token = token,
                expiresIn = totalSeconds // 權杖有效期限 (3個月)

            });
        }



        // 釋放資料庫連線資源
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}