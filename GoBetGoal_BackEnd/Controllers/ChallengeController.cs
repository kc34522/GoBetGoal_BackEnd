// 1. 確保檔案頂部有這些 using 指示詞
using GoBetGoal_BackEnd.Controllers;
using GoBetGoal_BackEnd.Models;
using GoBetGoal_BackEnd.Models.DTOs;
using GoBetGoal_BackEnd.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;


// --- 2. 為了簡單且自給自足，我們將所有需要的 DTO 模型都定義在這裡 ---
// 這些都不需要在資料庫建表
#region API Data Models (DTOs)

/// <summary>
/// 前端提交關卡挑戰時，傳送給後端的資料格式
/// </summary>
public class ChallengeSubmissionRequest
{
    public int TrialId { get; set; }
    public int StageIndex { get; set; }
    public List<string> ImageUrls { get; set; }
}

/// <summary>
/// 後端完成審核後，回傳給前端的詳細結果
/// </summary>
public class ChallengeSubmissionResponse
{
    public bool OverallResult { get; set; }
    public string OverallMessage { get; set; }
    public List<ImageResult> ImageResults { get; set; } = new List<ImageResult>();

    public int ChanceRemain { get; set; }// 每次審核後-1
}

/// <summary>
/// 單張圖片的審核結果
/// </summary>
public class ImageResult
{
    public string ImageUrl { get; set; }
    public bool IsSafe { get; set; }

    public bool IsCompliant { get; set; }
    public string Reason { get; set; }
}

/// <summary>
/// 用於解析 AI 回應的專用模型
/// </summary>
public class AIVerificationResult
{
    [JsonProperty("safety_rating")]
    public string SafetyRating { get; set; }

    [JsonProperty("compliance_rating")]
    public string ComplianceRating { get; set; }

    [JsonProperty("reason")]
    public string Reason { get; set; }

    [JsonProperty("confidence")]
    public double? Confidence { get; set; } // 新增：AI 置信度
}

public class AICollectiveDetectionResult
{
    [JsonProperty("detected_matching_foods")]
    public List<string> DetectedMatchingFoods { get; set; }
    [JsonProperty("violated_rules")]
    public List<string> ViolatedRules { get; set; }
}
// 圖片+文字用
//public class VerificationRequest
//{
//    public List<string> ImageUrls { get; set; } = new List<string>();
//    public string Text { get; set; }  // 可選，用於文字審核
//}



// --- OpenAI API 請求的模型 ---
public class OpenAIChatRequest { [JsonProperty("model")] public string Model { get; set; } [JsonProperty("messages")] public List<RequestMessage> Messages { get; set; } [JsonProperty("max_tokens")] public int MaxTokens { get; set; } }
public class RequestMessage { [JsonProperty("role")] public string Role { get; set; } [JsonProperty("content")] public List<RequestContent> Content { get; set; } }
public class RequestContent { [JsonProperty("type")] public string Type { get; set; } [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)] public string Text { get; set; } [JsonProperty("image_url", NullValueHandling = NullValueHandling.Ignore)] public RequestImageUrl ImageUrl { get; set; } }
public class RequestImageUrl { [JsonProperty("url")] public string Url { get; set; } }
public class OpenAIChatResponse { [JsonProperty("choices")] public List<ResponseChoice> Choices { get; set; } }
public class ResponseChoice { [JsonProperty("message")] public ResponseMessage Message { get; set; } }
public class ResponseMessage { [JsonProperty("role")] public string Role { get; set; } [JsonProperty("content")] public JToken Content { get; set; } }

#endregion


// 3. 主要的 Controller 類別
public class ChallengeController : BaseApiController
{
    private readonly ChallengeDbService _dbService = new ChallengeDbService();


    [HttpPost, Route("api/challenge/submit"), AllowAnonymous]
    public async Task<IHttpActionResult> SubmitChallengeStage([FromBody] ChallengeSubmissionRequest request)
    {

        if (request == null || request.ImageUrls == null || !request.ImageUrls.Any())
            return BadRequest("請求格式錯誤或未提供圖片。");

        Guid currentUserId = GetCurrentUserId();

        using (var _db = new Context())
        {
            var userStage = await _db.UserStages
            .Include("Stage.TrialTemplate").
            FirstOrDefaultAsync(us => us.UserId == currentUserId &&
            us.TrialId == request.TrialId &&
            us.Stage.StageIndex == request.StageIndex);

            if (userStage == null)
            {
                return NotFound();
            }

            if(userStage.ChanceRemain <= 0)
            {
                return BadRequest("您今日的 AI 審核次數已用完。");
            }

            var stageRules = userStage.Stage.StageDescription;
            var trialRules = userStage.Stage.TrialTemplate.TrialRule;

            if (stageRules == null || trialRules==null)
            {
                return InternalServerError(new Exception("關卡規則資料不完整。"));
            }

            var stageRulesJson = JsonConvert.DeserializeObject<List<string>>(stageRules);
            var trialRulesJson = JsonConvert.DeserializeObject<List<string>>(trialRules);

            IVerifier verifier = (userStage.Stage.VerificationMode == "Collective")
                ? (IVerifier)new CollectiveVerifier(userStage.Stage)
                : new PerImageVerifier(userStage.Stage);

            // --- D. 執行審核 ---
            // 將審核任務委派給選擇好的審核器去執行。
            var result = await verifier.VerifyAsync(request.ImageUrls);

            // --- 7. 處理審核結果 (更新 userStage 物件) ---
            userStage.ChanceRemain--; // 無論成敗，都扣除一次機會

            if (result.OverallResult)
            {
                userStage.UploadImagePath = JsonConvert.SerializeObject(request.ImageUrls);
                userStage.ImageUploadAt = DateTime.Now;
                userStage.Status = (GoBetGoal_BackEnd.Enums.Status)2;
            }

            _db.Entry(userStage).State=EntityState.Modified;
            await _db.SaveChangesAsync();

            return Ok(result);
        }
        // --- E. 如果所有圖片都失敗，嘗試備用審核策略 ---
        //if (!result.OverallResult && result.ImageResults.All(r => !r.IsSafe))
        //{
        //    var fallbackResult = await TryFallbackVerification(request.ImageUrls, stage);
        //    if (fallbackResult.OverallResult)
        //    {
        //        result = fallbackResult;
        //        result.OverallMessage += " (使用備用審核通過)";
        //    }
        //}
    }

    #region Verifier Strategy Pattern & Helpers (審核策略模式與所有輔助方法的完整實作)

    /// <summary>
    /// 定義所有審核器都必須遵守的合約（介面）
    /// </summary>
    public interface IVerifier
    {
        Task<ChallengeSubmissionResponse> VerifyAsync(List<string> imageUrls);

        // 文字+圖片審核用
        //Task<ChallengeSubmissionResponse> VerifyAsync(VerificationRequest request);

    }

    /// <summary>
    /// 「逐張獨立審核」的專科醫生
    /// </summary>
    public class PerImageVerifier : IVerifier
    {
        private readonly Stage _stage; // 儲存從資料庫撈出來的、包含所有規則的關卡資訊

        public PerImageVerifier(Stage stage)
        {
            _stage = stage;
        }

        public async Task<ChallengeSubmissionResponse> VerifyAsync(List<string> imageUrls)
        {
            var response = new ChallengeSubmissionResponse();

            // ... (數量檢查的防呆機制) ...

            bool isStagePassed = true;

            // 反序列化從資料庫來的 JSON 字串規則
            var mealRules = JsonConvert.DeserializeObject<List<string>>(_stage.StageDescription);
            var generalRules = JsonConvert.DeserializeObject<List<string>>(_stage.TrialTemplate.TrialRule);

            if (mealRules.Count > 1 && imageUrls.Count != mealRules.Count)
            {
                response.OverallResult = false;
                response.OverallMessage = $"驗證失敗：此關卡需要 {mealRules.Count} 張照片，但您提供了 {imageUrls.Count} 張。";
                return response;
            }

            // PerImage 模式：對每一張圖獨立進行審核
            for (int i = 0; i < imageUrls.Count; i++)
            {
                var imageUrl = imageUrls[i];
                // 如果規則數與圖片數對應，則按順序取規則；否則，所有圖片都使用第一條規則。
                string ruleForThisImage = (mealRules.Count > i) ? mealRules[i] : mealRules.FirstOrDefault() ?? "";

                string prompt = ChallengeHelper.BuildPerImagePrompt(ruleForThisImage, generalRules);

                // 呼叫 OpenAI 服務，建議使用 gpt-4o 以獲得更精準的邏輯判斷
                string rawAiResponse = await OpenAIHttpClientService.AnalyzeImageAsync(imageUrl, "gpt-4o", prompt);
                var result = ChallengeHelper.ParseAIResponse<AIVerificationResult>(rawAiResponse);

                // --- [新的、更穩健的後端判斷邏輯] ---
                bool isSafe = result.SafetyRating?.Equals("safe", StringComparison.OrdinalIgnoreCase) ?? false;
                bool isCompliant = result.ComplianceRating?.Equals("compliant", StringComparison.OrdinalIgnoreCase) ?? false;

                // 優先檢查安全性
                if (!isSafe)
                {
                    isStagePassed = false;
                    response.ImageResults.Add(new ImageResult { ImageUrl = imageUrl, IsSafe = false, IsCompliant = false, Reason = "圖片內容不符合社群安全規範。" });
                }
                else
                {
                    // 安全才檢查任務符合性
                    response.ImageResults.Add(new ImageResult { ImageUrl = imageUrl, IsSafe = true, IsCompliant = isCompliant, Reason = result.Reason });
                    if (!isCompliant) isStagePassed = false;
                }
            }
            response.OverallResult = isStagePassed;
            response.OverallMessage = isStagePassed ? "所有圖片均通過審核！" : "有圖片未通過審核，挑戰失敗。";
            return response;
        }
    }

    /// <summary>
    /// 「多圖綜合審核」的專科醫生。
    /// 適用於：哈佛 22-28 天自由餐等特殊任務。
    /// </summary>
    public class CollectiveVerifier : IVerifier
    {
        private readonly Stage _stage;

        public CollectiveVerifier(Stage stage)
        {
            _stage = stage;
        }

        public async Task<ChallengeSubmissionResponse> VerifyAsync(List<string> imageUrls)
        {
            var response = new ChallengeSubmissionResponse { ImageResults = new List<ImageResult>() };
            var allDetectedFoods = new List<string>();
            var allViolations = new List<string>();
            bool violationsFound = false;

            string mainRule = JsonConvert.DeserializeObject<List<string>>(_stage.StageDescription).FirstOrDefault() ?? "";
            var generalRules = JsonConvert.DeserializeObject<List<string>>(_stage.TrialTemplate.TrialRule);

            foreach (var imageUrl in imageUrls)
            {
                string prompt = ChallengeHelper.BuildCollectivePrompt(mainRule, generalRules);
                string rawAiResponse = await OpenAIHttpClientService.AnalyzeImageAsync(imageUrl, "gpt-4o", prompt);
                var result = ChallengeHelper.ParseAIResponse<AICollectiveDetectionResult>(rawAiResponse);

                if (result.DetectedMatchingFoods != null) allDetectedFoods.AddRange(result.DetectedMatchingFoods);

                bool hasViolation = result.ViolatedRules != null && result.ViolatedRules.Any();
                if (hasViolation) { violationsFound = true; allViolations.AddRange(result.ViolatedRules); }

                response.ImageResults.Add(new ImageResult
                {
                    ImageUrl = imageUrl,
                    IsCompliant = !hasViolation,
                    Reason = hasViolation ? "違反規則：" + string.Join(", ", result.ViolatedRules) : "偵測到的合規食物：" + string.Join(", ", result.DetectedMatchingFoods)
                });
            }

            bool allItemsMet = ChallengeHelper.CheckFreeMealCompliance(allDetectedFoods, mainRule);
            response.OverallResult = allItemsMet && !violationsFound;
            response.OverallMessage = response.OverallResult ? "恭喜！您已完成今日所需的所有餐點！" : "審核失敗，請確認是否缺少必要食物或違反了某些規則。";
            return response;
        }
    }
    #endregion

    //public class AiVerifier : IVerifier
    //{
    //    public async Task<ChallengeSubmissionResponse> VerifyAsync(VerificationRequest request)
    //    {
    //        // 呼叫 AI API 做圖片 + 文字審核
    //        return new ChallengeSubmissionResponse
    //        {
    //            OverallResult = true,
    //            OverallMessage = 
    //        };
    //    }
    //}


    // 模擬人工審核：假設所有圖片都合規
    //public class ManualVerifier : IVerifier
    //{
    //    // 不需要非同步 → 移除 async
    //    public Task<ChallengeSubmissionResponse> VerifyAsync(List<string> imageUrls)
    //    {
    //        // 模擬人工審核：假設所有圖片都合規
    //        var imageResults = imageUrls.Select(url => new ImageResult
    //        {
    //            ImageUrl = url,
    //            IsSafe = true,
    //            IsCompliant = true,
    //            Reason = "人工審核通過"
    //        }).ToList();

    //        var response = new ChallengeSubmissionResponse
    //        {
    //            OverallResult = true,
    //            OverallMessage = "所有圖片皆通過人工審核",
    //            ImageResults = imageResults
    //        };

    //        return Task.FromResult(response);
    //    }
    //}
}