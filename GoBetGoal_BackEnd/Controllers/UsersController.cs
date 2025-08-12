using GoBetGoal_BackEnd.Models;
using GoBetGoal_BackEnd.Models.DTOs;
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
                var errorResponse = new
                {
                    errorCode = "NICKNAME_ALREADY_EXISTS",
                    message = "此暱稱已被使用，請嘗試使用其他暱稱註冊。"
                };

                return Content(HttpStatusCode.Conflict, errorResponse);

            }

            var selectedAvatar = _db.Avatars.FirstOrDefault(a => a.Id == model.AvatarId && a.IsActive);
            if (selectedAvatar == null)
            {
                var errorResponse = new
                {
                    errorCode = "AVATAR_NOT_FOUND",
                    message = "指定的頭像不存在。"
                };

                return Content(HttpStatusCode.BadRequest, errorResponse);
            }

            if (selectedAvatar.AvatarPrice > 0)
            {
                bool userOwnsThisAvatar = _db.UserAvatars.Any(a => a.UserId == currentUserId && a.AvatarId == model.AvatarId);

                if (!userOwnsThisAvatar)
                {
                    var errorResponse = new
                    {
                        errorCode = "AVATAR_NOT_OWNED",
                        message = "無法選擇尚未擁有的付費頭像。"
                    };

                    return Content(HttpStatusCode.Forbidden, errorResponse);

                }
            }

            var userToUpdate = _db.Users.Find(currentUserId);
            if (userToUpdate == null)
            {
                return NotFound();
            }

            userToUpdate.NickName = model.NickName;
            userToUpdate.UpdatedAt = DateTime.Now;

            var userAvatarsToUpdate = _db.UserAvatars.FirstOrDefault(a => a.UserId == currentUserId && a.AvatarId == model.AvatarId);

            if (userAvatarsToUpdate != null)
            {
                userAvatarsToUpdate.IsCurrent = true;
            }

            try
            {
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            return Ok(new { message = "個人資料建立成功！" });

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