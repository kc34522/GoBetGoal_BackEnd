using GoBetGoal_BackEnd.Filters;
using GoBetGoal_BackEnd.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace GoBetGoal_BackEnd
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //// 啟用 CORS
            //// 這裡的 "origins: "*", headers: "*", methods: "*"" 表示允許任何來源、任何標頭、任何方法的請求
            //// 在正式上線時，建議將 origins 改為您前端的實際網址，例如 "http://your-frontend-domain.com"
            //var cors = new EnableCorsAttribute("*", "*", "*");
            //config.EnableCors(cors);

            // 2. 忽略值為 null 的屬性
            //jsonSettings.NullValueHandling = NullValueHandling.Ignore;

            // 3. 避免循環參考造成的錯誤
            //jsonSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            // --- 移除 XML Formatter ---
            // 讓 API 只回傳 JSON 格式
            //config.Formatters.Remove(config.Formatters.XmlFormatter);

            // 將我們寫好的 GlobalExceptionFilter 註冊為全局過濾器
            config.Filters.Add(new GlobalExceptionFilter());

            // Web API 設定和服務

            // *** 將您的 JwtAuthFilter 註冊為全局過濾器 ***
            // 從現在開始，每一支 API 預設都需要通過 JWT 驗證
            config.Filters.Add(new JwtAuthFilter());

            // --- 加入這段設定來自動轉換命名風格 ---
            var jsonFormatter = config.Formatters.JsonFormatter;
            // 就是這一行，告訴框架：把所有 PascalCase 的屬性，在序列化成 JSON 時，都轉成 camelCase
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
