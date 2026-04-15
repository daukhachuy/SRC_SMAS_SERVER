using Microsoft.Extensions.Options;
using SMAS_BusinessObject.DTOs.AIDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SMAS_Services.AiBaseServices
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly GeminiSettings _settings;

        public AIService(HttpClient httpClient, IOptions<GeminiSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<string> AskAI(string prompt)
        {
            //var url = $"https://generativelanguage.googleapis.com/v1/models/{_settings.Model}:generateContent?key={_settings.ApiKey}";
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_settings.Model}:generateContent?key={_settings.ApiKey}";
            var requestBody = new
            {
                contents = new[]
                {
            new
            {
                parts = new[]
                {
                    new { text = prompt }
                }
            }
        }
            };

            var response = await _httpClient.PostAsJsonAsync(url, requestBody);

            var result = await response.Content.ReadAsStringAsync();

            Console.WriteLine("===== GEMINI RAW =====");
            Console.WriteLine(result);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Gemini API error: {result}");

            return ExtractText(result);
        }

        private string ExtractText(string json)
        {
            using var doc = JsonDocument.Parse(json);

            var candidates = doc.RootElement.GetProperty("candidates");

            if (candidates.GetArrayLength() == 0)
                throw new Exception("AI không có candidates");

            var parts = candidates[0]
                .GetProperty("content")
                .GetProperty("parts");

            foreach (var part in parts.EnumerateArray())
            {
                if (part.TryGetProperty("text", out var textElement))
                {
                    var text = textElement.GetString();

                    if (!string.IsNullOrEmpty(text))
                        return CleanJson(text);
                }
            }

            throw new Exception("AI không trả text hợp lệ");
        }

        private string CleanJson(string text)
        {
            // remove markdown
            text = text.Replace("```json", "")
                       .Replace("```", "")
                       .Trim();

            var start = text.IndexOf("{");
            var end = text.LastIndexOf("}");

            if (start >= 0 && end > start)
            {
                return text.Substring(start, end - start + 1);
            }

            throw new Exception("Không tìm thấy JSON hợp lệ từ AI");
        }
    }
}