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

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Gemini API error: {response.StatusCode}");
            }

            var json = await response.Content.ReadAsStringAsync();

            // 👉 extract text từ Gemini
            return ExtractText(json);
        }

        // 🔥 Hàm QUAN TRỌNG
        private string ExtractText(string json)
        {
            var gemini = JsonSerializer.Deserialize<GeminiResponse>(json);

            var text = gemini?.Candidates?[0]?.Content?.Parts?[0]?.Text;

            if (string.IsNullOrEmpty(text))
                throw new Exception("AI không trả dữ liệu");

            // 👉 cleanup nếu AI trả thêm text ngoài JSON
            return CleanJson(text);
        }

        // 🔥 Fix lỗi AI trả sai format
        private string CleanJson(string text)
        {
            var start = text.IndexOf("{");
            var end = text.LastIndexOf("}");

            if (start >= 0 && end > start)
            {
                return text.Substring(start, end - start + 1);
            }

            // fallback
            return text;
        }
    }
}