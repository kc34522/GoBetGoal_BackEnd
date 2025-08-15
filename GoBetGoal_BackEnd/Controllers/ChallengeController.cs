// 1. 確保檔案頂部有這些 using 指示詞
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// 2. 為了簡單，我們將所有需要的模型都定義在 Controller 檔案的外部
#region Data Models (所有需要的資料模型)

// --- API 請求與回應的模型 ---
public class ChallengeSubmissionRequest
{
    public int StageId { get; set; }
    public List<string> ImageUrls { get; set; }
}

public class ChallengeSubmissionResponse
{
    public bool OverallResult { get; set; }
    public string OverallMessage { get; set; }
    public List<ImageResult> ImageResults { get; set; }
}

public class ImageResult
{
    public string ImageUrl { get; set; }
    public bool IsCompliant { get; set; }
    public string Reason { get; set; }
}

// --- 邏輯層/資料庫的模型 ---
public class StageInfo
{
    public int StageId { get; set; }
    public List<string> MealRules { get; set; }
    public List<string> GeneralRules { get; set; }
    public string VerificationMode { get; set; }
}

// --- 解析 AI 回應的專用模型 ---
public class AIVerificationResult
{
    [JsonProperty("is_compliant")]
    public bool IsCompliant { get; set; }
    [JsonProperty("reason")]
    public string Reason { get; set; }
}

public class AICollectiveDetectionResult
{
    [JsonProperty("detected_matching_foods")]
    public List<string> DetectedMatchingFoods { get; set; }
    [JsonProperty("violated_rules")]
    public List<string> ViolatedRules { get; set; }
}

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
[RoutePrefix("api/challenge")]
public class ChallengeController : ApiController
{
    [HttpPost, Route("submit"), AllowAnonymous]
    public async Task<IHttpActionResult> SubmitChallengeStage([FromBody] ChallengeSubmissionRequest request)
    {
        if (request == null || request.ImageUrls == null || !request.ImageUrls.Any())
            return BadRequest("缺少必要參數");

        var stageInfo = GetMockStageInfo(request.StageId);
        if (stageInfo == null) return NotFound();

        IVerifier verifier = (stageInfo.VerificationMode == "Collective")
            ? (IVerifier)new CollectiveVerifier()
            : new PerImageVerifier();

        var result = await verifier.VerifyAsync(request, stageInfo);
        return Ok(result);
    }

    #region Verifier Strategy Pattern, Helpers, and AI Service

    // --- 策略模式 ---
    private interface IVerifier { Task<ChallengeSubmissionResponse> VerifyAsync(ChallengeSubmissionRequest request, StageInfo stageInfo); }

    private class PerImageVerifier : IVerifier
    {
        public async Task<ChallengeSubmissionResponse> VerifyAsync(ChallengeSubmissionRequest request, StageInfo stageInfo)
        {
            var response = new ChallengeSubmissionResponse { ImageResults = new List<ImageResult>() };
            bool isStagePassed = true;

            for (int i = 0; i < request.ImageUrls.Count; i++)
            {
                var imageUrl = request.ImageUrls[i];
                string ruleForThisImage = (stageInfo.MealRules.Count > i) ? stageInfo.MealRules[i] : stageInfo.MealRules.FirstOrDefault() ?? "";
                string prompt = BuildPerImagePrompt(ruleForThisImage, stageInfo.GeneralRules);
                string rawAiResponse = await OpenAIHttpClientService.AnalyzeImageAsync(imageUrl, "gpt-4o-mini", prompt);
                var result = ParseAIResponse<AIVerificationResult>(rawAiResponse);

                response.ImageResults.Add(new ImageResult { ImageUrl = imageUrl, IsCompliant = result.IsCompliant, Reason = result.Reason });
                if (!result.IsCompliant) isStagePassed = false;
            }
            response.OverallResult = isStagePassed;
            response.OverallMessage = isStagePassed ? "所有圖片均通過審核！" : "有圖片未通過審核，挑戰失敗。";
            return response;
        }
    }

    private class CollectiveVerifier : IVerifier
    {
        public async Task<ChallengeSubmissionResponse> VerifyAsync(ChallengeSubmissionRequest request, StageInfo stageInfo)
        {
            var response = new ChallengeSubmissionResponse { ImageResults = new List<ImageResult>() };
            var allDetectedFoods = new List<string>();
            var allViolations = new List<string>();
            bool violationsFound = false;
            string mainRule = stageInfo.MealRules.FirstOrDefault() ?? "";

            foreach (var imageUrl in request.ImageUrls)
            {
                string prompt = BuildCollectivePrompt(mainRule, stageInfo.GeneralRules);
                string rawAiResponse = await OpenAIHttpClientService.AnalyzeImageAsync(imageUrl, "gpt-4o", prompt);
                var result = ParseAIResponse<AICollectiveDetectionResult>(rawAiResponse);

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
            bool allItemsMet = CheckFreeMealCompliance(allDetectedFoods, mainRule);
            response.OverallResult = allItemsMet && !violationsFound;
            response.OverallMessage = response.OverallResult ? "恭喜！您已完成今日所需的所有餐點！" : "審核失敗，請確認是否缺少必要食物或違反了某些規則。";
            return response;
        }
    }

    // --- AI 服務 (直接放在這裡，最簡單) ---
    private static class OpenAIHttpClientService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string OpenAI_Api_Url = "https://api.openai.com/v1/chat/completions";

        public static Task<string> AnalyzeImageAsync(string imageUrl, string model, string prompt)
        {
            return AnalyzeImagesInternal(new List<string> { imageUrl }, model, prompt);
        }

        private static async Task<string> AnalyzeImagesInternal(List<string> imageUrls, string model, string prompt)
        {
            var apiKey = ConfigurationManager.AppSettings["OpenAI_ApiKey"];
            if (string.IsNullOrEmpty(apiKey) || apiKey.StartsWith("sk-YOUR")) throw new ArgumentException("Web.config 中未設定 OpenAI_ApiKey。");

            var contentList = new List<RequestContent> { new RequestContent { Type = "text", Text = prompt } };
            imageUrls.ForEach(url => contentList.Add(new RequestContent { Type = "image_url", ImageUrl = new RequestImageUrl { Url = url } }));

            var requestBody = new OpenAIChatRequest { Model = model, MaxTokens = 500, Messages = new List<RequestMessage> { new RequestMessage { Role = "user", Content = contentList } } };

            try
            {
                var jsonContent = JsonConvert.SerializeObject(requestBody);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, OpenAI_Api_Url))
                {
                    httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                    httpRequestMessage.Content = httpContent;
                    var response = await _httpClient.SendAsync(httpRequestMessage);
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode) return JsonConvert.SerializeObject(new { is_compliant = false, reason = $"API 請求失敗: {response.StatusCode}" });
                    var openAIResponse = JsonConvert.DeserializeObject<OpenAIChatResponse>(jsonResponse);
                    var message = openAIResponse?.Choices?.FirstOrDefault()?.Message;
                    if (message == null) return "{\"is_compliant\": false, \"reason\": \"AI 未提供有效回應。\"}";
                    return message.Content.ToString();
                }
            }
            catch (Exception ex) { return $"{{\"is_compliant\": false, \"reason\": \"程式執行異常：{ex.Message}\"}}"; }
        }
    }

    // --- 所有輔助方法 ---
    private static StageInfo GetMockStageInfo(int stageId)
    {
        switch (stageId)
        {
            case 1: return new StageInfo { StageId = 1, VerificationMode = "PerImage", MealRules = new List<string> { "半顆葡萄柚或橘子＋1或2顆水煮蛋", "水果吃到飽", "煎或水煮雞胸肉" }, GeneralRules = new List<string> { "不可替換食材", "水煮蔬菜不可加油" } };
            case 22: return new StageInfo { StageId = 22, VerificationMode = "Collective", MealRules = new List<string> { "自由餐：250g雞胸肉＋3顆番茄" }, GeneralRules = new List<string> { "不可替換食材", "水煮蔬菜不可加油" } };
            default: return null;
        }
    }

    private static string BuildPerImagePrompt(string rule, List<string> generalRules)
    {
        return $"You are a strict judge for a health challenge. Analyze this single image. The image MUST comply with this specific rule: '{rule}'. You must ALSO enforce these general guidelines: {string.Join(", ", generalRules)}. Respond ONLY with a JSON object in Traditional Chinese in the format: {{\"is_compliant\": boolean, \"reason\": \"A brief explanation.\"}}";
    }

    private static string BuildCollectivePrompt(string rule, List<string> generalRules)
    {
        return $"You are a food detective. The complete daily requirement is: '{rule}'. Analyze THIS SINGLE IMAGE and identify ONLY the food items that are part of the daily requirement list. Also, report any violations of these general guidelines: {string.Join(", ", generalRules)}. Respond ONLY with a JSON object in Traditional Chinese: {{\"detected_matching_foods\": [\"food1\"], \"violated_rules\": [\"violation reason\"]}}";
    }

    private static T ParseAIResponse<T>(string rawResponse) where T : class, new()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(rawResponse)) return new T();
            var cleanJson = rawResponse.Trim().Trim('`').Replace("json\n", "").Trim();
            return JsonConvert.DeserializeObject<T>(cleanJson) ?? new T();
        }
        catch { return new T(); }
    }

    private static bool CheckFreeMealCompliance(List<string> detectedFoods, string rule)
    {
        // 這是簡化的檢查邏輯，您可以擴充它
        return rule.Contains("雞胸肉") && detectedFoods.Any(f => f.Contains("雞胸肉"));
    }
    #endregion
}