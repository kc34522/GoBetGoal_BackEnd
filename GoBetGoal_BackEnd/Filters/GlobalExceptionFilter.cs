using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace GoBetGoal_BackEnd.Filters
{
    public class GlobalExceptionFilter:ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            // 取得發生例外的原因
            var exception = actionExecutedContext.Exception;

            // 1. (對內) 記錄錯誤 Log
            // 在正式專案中，這裡會使用 NLog, Serilog 等日誌框架
            // 在學習階段，我們可以用 System.Diagnostics.Debug.WriteLine 來將完整錯誤印在 Visual Studio 的「輸出」視窗中，方便偵錯
            System.Diagnostics.Debug.WriteLine($"[GlobalExceptionFilter] An unhandled exception occurred: {exception}");

            // 2. (對外) 準備一個乾淨、通用的錯誤回應
            var errorResponse = new
            {
                message = "伺服器發生未預期的錯誤，請聯繫系統管理員或稍後再試。"
            };

            // --- (可選) 針對特定錯誤類型，給予稍微不同的通用訊息 ---
            if (exception is DbUpdateException)
            {
                // 如果是資料庫更新錯誤，可以給一個稍微不同的提示
                errorResponse = new { message = "資料庫更新失敗，請檢查您提交的資料或稍後再試。" };
            }
            // 您可以在這裡加入更多 else if 來處理其他特定類型的例外...
            // --------------------------------------------------

            // 3. 建立一個 HTTP 500 Internal Server Error 回應
            actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(
                HttpStatusCode.InternalServerError,
                errorResponse
            );

            base.OnException(actionExecutedContext);
        }
    }
}