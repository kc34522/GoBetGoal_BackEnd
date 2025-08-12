using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace GoBetGoal_BackEnd.Controllers
{
    // 讓這個基底類別繼承自 ApiController
    public abstract class BaseApiController : ApiController
    {
        /// <summary>
        /// 從 JWT Payload 中取得當前登入者的 User ID。
        /// 如果取得失敗，會直接拋出一個 401 Unauthorized 錯誤。
        /// </summary>
        /// <returns>當前使用者的 Guid</returns>
        protected Guid GetCurrentUserId()
        {
            // 將我們之前寫的、繁瑣但安全的邏輯，全部搬到這裡
            if (Request.Properties.TryGetValue("jwtPayload", out object payloadObject))
            {
                var payload = payloadObject as Dictionary<string, object>;
                if (payload != null && payload.ContainsKey("Id"))
                {
                    string userIdString = payload["Id"].ToString();
                    if (Guid.TryParse(userIdString, out Guid currentUserId))
                    {
                        // 成功取得，回傳 Guid
                        return currentUserId;
                    }
                }
            }

            // 如果以上任何一個步驟失敗，就代表 Token 有問題，直接拋出 401 錯誤
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
    }
}