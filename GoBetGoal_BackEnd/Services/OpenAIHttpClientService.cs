using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GoBetGoal_BackEnd.Services
{
    public static class OpenAIHttpClientService
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
}