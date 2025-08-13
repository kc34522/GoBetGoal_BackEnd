using GoBetGoal_BackEnd.Models;
using GoBetGoal_BackEnd.Models.DTOs;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace GoBetGoal_BackEnd.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly Context _db = new Context();
        /// <summary>
        /// 註冊第二步：初次設定個人資料
        /// </summary>

        [HttpPost]
        [Route("api/users/me/profile")]
        public IHttpActionResult CompleteUserProfile(RegisterStepTwoRequestDto model)
        {
            Guid currentUserId = GetCurrentUserId();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool IsNickNameTaken = _db.Users.Any(u => u.NickName == model.NickName);
            if (IsNickNameTaken)
            {
                var error = new ErrorResponseDto
                {
                    ErrorCode = "NICKNAME_ALREADY_EXISTS",
                    Message = "此暱稱已被使用，請嘗試使用其他暱稱註冊。"
                };

                return Content(HttpStatusCode.Conflict, error);

            }

            var selectedAvatar = _db.Avatars.FirstOrDefault(a => a.Id == model.AvatarId && a.IsActive);
            if (selectedAvatar == null)
            {
                var error = new ErrorResponseDto
                {
                    ErrorCode = "AVATAR_NOT_FOUND",
                    Message = "指定的頭像不存在。"
                };

                return Content(HttpStatusCode.BadRequest, error);
            }

            if (selectedAvatar.AvatarPrice > 0)
            {
                bool userOwnsThisAvatar = _db.UserAvatars.Any(a => a.UserId == currentUserId && a.AvatarId == model.AvatarId);

                if (!userOwnsThisAvatar)
                {
                    var error = new ErrorResponseDto
                    {
                        ErrorCode = "AVATAR_NOT_OWNED",
                        Message = "無法選擇尚未擁有的付費頭像。"
                    };

                    return Content(HttpStatusCode.Forbidden, error);

                }
            }

            var userToUpdate = _db.Users.Find(currentUserId);
            if (userToUpdate == null)
            {
                return NotFound();
            }

            userToUpdate.NickName = model.NickName;
            userToUpdate.UpdatedAt = DateTime.Now;
            userToUpdate.BagelCount += 10000;

            // 2. 更新 UserAvatar 表
            //    a. 先將使用者目前所有的頭像都設為非當前 (IsCurrent = false)
            var allUserAvatars = _db.UserAvatars.Where(ua => ua.UserId == currentUserId);
            foreach (var avatarEntry in allUserAvatars)
            {
                avatarEntry.IsCurrent = false;
            }

            //    b. 接著，從 allUserAvatars 中找到使用者「新選擇」的那一筆
            var chosenAvatarEntry = allUserAvatars.FirstOrDefault(ua => ua.AvatarId == model.AvatarId);

            //    c. 將它設為當前 (IsCurrent = true)
            //       (我們在前面的驗證已確保 chosenAvatarEntry 不會是 null，所以這裡可以直接設定)
            if (chosenAvatarEntry != null)
            {
                chosenAvatarEntry.IsCurrent = true;
            }
            else
            {
                // 這是一個保險措施，理論上前段驗證不會讓使用者選到他沒有的頭像
                // 但如果真的發生了，回傳一個錯誤
                var error = new ErrorResponseDto { ErrorCode = "AVATAR_NOT_OWNED", Message = "無法選擇尚未擁有的付費頭像。" };
                return Content(HttpStatusCode.BadRequest, error);
            }



            try
            {
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            var successResponse = new SuccessResponseDto { Message = "個人資料建立成功！恭喜您獲得 10,000 貝果獎勵！" };
            return Ok(successResponse);



        }

        [HttpGet]
        [Route("api/users/me")]
        public IHttpActionResult GetMyProfile()
        {
            Guid currentUserId = GetCurrentUserId();

            var userProfile = _db.Users
                .Where(a => a.Id == currentUserId)
                .Select(a => new UserProfileDto
                {
                    Id = a.Id,
                    Email = a.Email,
                    PlayerId = a.PlayerId,
                    NickName = a.NickName,
                    BagelCount = a.BagelCount,
                    CheatBlanketCount = a.CheatBlanketCount,
                    ColorMode = a.ColorModeType,
                    CreatedAt = a.CreatedAt,
                    CurrentAvatarId = a.UserAvatars.Where(u => u.IsCurrent).Select(u => (int?)u.AvatarId).FirstOrDefault(),
                    CurrentAvatarUrl = a.UserAvatars.Where(u => u.IsCurrent).Select(u => u.Avatar.AvatarImagePath).FirstOrDefault()
                }).FirstOrDefault();

            if (userProfile == null)
            {
                return NotFound();
            }

            return Ok(userProfile);
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