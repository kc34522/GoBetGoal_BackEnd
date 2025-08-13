using GoBetGoal_BackEnd.Models.DTOs;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace GoBetGoal_BackEnd.Security
{
    /// <summary>
    /// JwtAuthFilter 繼承 ActionFilterAttribute 可生成 [JwtAuthFilter] 標籤使用
    /// 在需要登入的api加上此標籤便會執行JwtAuthFilter，可加在controller or action(api)
    /// </summary>
    public class JwtAuthFilter : ActionFilterAttribute
    {
        // 加解密的 key，如果不一樣會無法成功解密
        private static readonly string secretKey = WebConfigurationManager.AppSettings["TokenKey"];

        /// <summary>
        /// 過濾有用標籤 [JwtAuthFilter] 請求的 API 的 JwtToken 狀態及內容
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            // 檢查 Action (方法) 上方是否有 [AllowAnonymous]
            bool isAnonymousAllowed = actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
            if (isAnonymousAllowed)
            {
                // 如果有，就直接跳過後續所有驗證邏輯
                return;
            }

            // 取出請求內容並排除不需要驗證的 API
            var request = actionContext.Request; // 取出整包請求
                                                 //if (!WithoutVerifyToken(request.RequestUri.ToString()))
                                                 //{
                                                 // 有取到 JwtToken 後，判斷授權格式不存在且不正確時
            if (request.Headers.Authorization == null || request.Headers.Authorization.Scheme != "Bearer" || string.IsNullOrEmpty(request.Headers.Authorization.Parameter))
            {
                // 可考慮配合前端專案開發期限，不修改 StatusCode 預設 200，將請求失敗搭配 Status: false 供前端判斷
                //string messageJson = JsonConvert.SerializeObject(new { Status = false, Message = "請重新登入" }); // JwtToken 遺失，需導引重新登入

                var error = new ErrorResponseDto
                {
                    ErrorCode = "TOKEN_MISSING_OR_INVALID_FORMAT",
                    // 行動導向的統一訊息
                    Message = "連線階段無效或已過期，請重新登入。"
                    //Message = "驗證失敗：遺失 Token 或格式錯誤。"
                };
                actionContext.Response = request.CreateResponse(HttpStatusCode.Unauthorized, error);
                return;

                //var errorMessage = new HttpResponseMessage()
                //{
                //    //StatusCode = System.Net.HttpStatusCode.Unauthorized, // 401
                //    ReasonPhrase = "JwtToken Lost", // 在postman statuscod旁的錯誤訊息
                //    Content = new StringContent(messageJson,
                //                Encoding.UTF8,
                //                "application/json")
                //};
                //throw new HttpResponseException(errorMessage); // Debug 模式會停在此行，點繼續執行即可 // 本機測試環境會停在這裡 正式環境會拋出錯誤
            }


            try // 才不會錯誤訊息洩漏
            {
                // 有 JwtToken 且授權格式正確時執行，用 try 包住，因為如果有篡改可能解密失敗
                // 解密後會回傳 Json 格式的物件 (即加密前的資料)
                var jwtAuthUtil = new JwtAuthUtility();
                var payload = jwtAuthUtil.GetPayload(request.Headers.Authorization.Parameter);
                if (payload == null)
                {
                    // GetPayload 回傳 null 代表解碼失敗或簽章不符
                    var error = new ErrorResponseDto
                    {
                        ErrorCode = "TOKEN_INVALID",
                        // 行動導向的統一訊息
                        Message = "連線階段無效或已過期，請重新登入。"
                        //Message = "驗證失敗：Token 無效或簽章不符。"
                    };
                    actionContext.Response = request.CreateResponse(HttpStatusCode.Unauthorized, error);
                    return;
                }

                // 檢查有效期限是否過期，如 JwtToken 過期，需導引重新登入
                if (IsTokenExpired(payload["Exp"].ToString()))
                {
                    var error = new ErrorResponseDto
                    {
                        ErrorCode = "TOKEN_EXPIRED",
                        // 這裡可以給一個稍微不同的訊息，以增加一點點友好度，但行動是一樣的
                        Message = "連線階段已過期，請重新登入。"
                        //Message = "驗證失敗：Token 已過期。"
                    };
                    actionContext.Response = request.CreateResponse(HttpStatusCode.Unauthorized, error);

                    return;

                    //string messageJson = JsonConvert.SerializeObject(new { Status = false, Message = "請重新登入" }); // JwtToken 過期，需導引重新登入
                    //var errorMessage = new HttpResponseMessage()
                    //{
                    //    // StatusCode = System.Net.HttpStatusCode.Unauthorized, // 401
                    //    ReasonPhrase = "JwtToken Expired",
                    //    Content = new StringContent(messageJson,
                    //        Encoding.UTF8,
                    //        "application/json")
                    //};
                    //throw new HttpResponseException(errorMessage); // Debug 模式會停在此行，點繼續執行即可

                }
                actionContext.Request.Properties["jwtPayload"] = payload;

            }
            catch (Exception ex)
            {
                // 步驟 1：(對內) 將完整的錯誤細節記錄到您的日誌系統中
                // Logger.Error(ex.ToString()); 
                // 在沒有日誌系統的初期，可以用 Debug.WriteLine 來在偵錯時看到
                System.Diagnostics.Debug.WriteLine($"An unexpected error occurred in JwtAuthFilter: {ex}");

                // 對於 Filter 內部未預期的錯誤，我們仍然可以使用 GlobalExceptionFilter 的統一格式
                // 這裡可以簡化，讓全局 Filter 去處理
                throw; // 重新拋出例外，讓 GlobalExceptionFilter 捕捉
                // 步驟 2：(對外) 回傳一個通用的、不包含任何內部細節的錯誤訊息
                //actionContext.Response = request.CreateErrorResponse(
                //    HttpStatusCode.InternalServerError,
                //    "伺服器發生未預期的錯誤，請聯繫系統管理員。" // <-- 通用、安全的訊息
                
                //return;
                // 解密失敗
                //string messageJson = JsonConvert.SerializeObject(new { Status = false, Message = "請重新登入" }); // JwtToken 不符，需導引重新登入
                //var errorMessage = new HttpResponseMessage()
                //{
                //    // StatusCode = System.Net.HttpStatusCode.Unauthorized, // 401
                //    ReasonPhrase = "JwtToken NotMatch",
                //    Content = new StringContent(messageJson,
                //            Encoding.UTF8,
                //            "application/json")
                //};
                //throw new HttpResponseException(errorMessage); // Debug 模式會停在此行，點繼續執行即可
            }

            // 執行到這裡，代表通行證有效，可以繼續前進
            base.OnActionExecuting(actionContext);
        }


        /// <summary>
        /// 有在 Global 設定一律檢查 JwtToken 時才需設定排除，例如 Login 不需要驗證因為還沒有 token
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        //public bool WithoutVerifyToken(string requestUri)
        //{
        //    //if (requestUri.EndsWith("/login")) return true;
        //    return false;
        //}

        /// <summary>
        /// 驗證 token 時效
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public bool IsTokenExpired(string dateTimeString)
        {
            return Convert.ToDateTime(dateTimeString) < DateTime.Now;
        }
    }
}