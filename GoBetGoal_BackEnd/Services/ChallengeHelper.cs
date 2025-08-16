using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoBetGoal_BackEnd.Services
{
    public static class ChallengeHelper
    {
        /// <summary>
        /// 建立「逐張審核」模式的 Prompt。【已修正為最簡單的字串語法】
        /// </summary>
        public static string BuildPerImagePrompt(string rule, List<string> generalRules)
        {
            string generalRulesText = string.Join("\n- ", generalRules);
            return $@"
You are a helpful and fair AI health coach. Your first priority is community safety.
## Judging Process:
1.  **Safety Analysis:** First, analyze the image for unsafe content (nudity, violence, self-harm, etc.).
2.  **Compliance Analysis:** Second, analyze if the image complies with the meal and general rules.
## Rules:
- General: {generalRulesText}
- Specific Meal Rule: '{rule}'
Respond ONLY with a JSON object in Traditional Chinese with the following format.
{{
  ""safety_rating"": ""safe"" or ""unsafe"",
  ""compliance_rating"": ""compliant"" or ""non_compliant"",
  ""reason"": ""A brief, helpful explanation.""
}}";
        }

        /// <summary>
        /// 建立「綜合審核」模式的 Prompt。【已修正為最簡單的字串語法】
        /// </summary>
        public static string BuildCollectivePrompt(string rule, List<string> generalRules)
        {
            // 先定義好 JSON 格式範本
            string jsonFormat = @"{""detected_matching_foods"": [""food1""], ""violated_rules"": [""violation reason""]}";

            // 使用字串串接
            return "You are a food detective for a health challenge. The complete daily requirement is: '" + rule + "'.\n" +
                   "Analyze THIS SINGLE IMAGE and identify ONLY the food items that are part of the daily requirement list.\n" +
                   "Also, report any violations of these general guidelines: " + string.Join(", ", generalRules) + ".\n" +
                   "Respond ONLY with a JSON object in Traditional Chinese: " + jsonFormat;
        }

        // ... (ParseAIResponse 和 CheckFreeMealCompliance 方法保持不變) ...
        public static T ParseAIResponse<T>(string rawResponse) where T : class, new()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rawResponse)) return new T();
                var cleanJson = rawResponse.Trim().Trim('`').Replace("json\n", "").Trim();
                return JsonConvert.DeserializeObject<T>(cleanJson) ?? new T();
            }
            catch { return new T(); }
        }

        public static bool CheckFreeMealCompliance(List<string> detectedFoods, string rule)
        {
            var lowerCaseDetectedFoods = detectedFoods.Select(f => f.ToLower()).ToList();
            var requiredFoods = new List<string>();
            if (rule.Contains("雞胸肉")) requiredFoods.Add("chicken");
            if (rule.Contains("番茄")) requiredFoods.Add("tomato");

            foreach (var required in requiredFoods)
            {
                if (!lowerCaseDetectedFoods.Any(detected => detected.Contains(required)))
                {
                    return false;
                }
            }
            return true;
        }
    }
}