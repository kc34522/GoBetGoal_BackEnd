using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace GoBetGoal_BackEnd.Controllers
{
    // 這是為了讓前端能傳送圖片 URL 給我們
    public class ImageTestRequest
    {
        public string ImageUrl { get; set; }
    }

    public class AITestController : ApiController
    {
        //    // 我們將建立一個路由為 /api/AITest/AnalyzeImage 的 POST 端點
        //    [HttpPost]
        //    [Route("api/AITest/AnalyzeImage")]
        //    [AllowAnonymous]
        //    public async Task<IHttpActionResult> AnalyzeImage([FromBody] ImageTestRequest request)
        //    {
        //        // 檢查前端是否有傳來圖片 URL
        //        if (request == null || string.IsNullOrEmpty(request.ImageUrl))
        //        {
        //            return BadRequest("請提供圖片的 URL。");
        //        }

        //        // 呼叫我們之前建立好的 OpenAIHttpClientService 來執行分析
        //        // 這裡我們直接使用 gpt-4o-mini 來測試，速度快且便宜
        //        string analysisResult = await OpenAIHttpClientService.AnalyzeImageAsync(request.ImageUrl, "gpt-4o-mini");

        //        // 將 AI 回傳的原始分析結果直接回傳給測試工具
        //        return Ok(new
        //        {
        //            SourceImageUrl = request.ImageUrl,
        //            AI_Response = analysisResult
        //        });
        //    }



    }
}