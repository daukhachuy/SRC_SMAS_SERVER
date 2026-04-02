using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.DTOs.AIDTO;
using SMAS_BusinessObject.Models;
using SMAS_Repositories.CustomerFeedbackRepositories;
using SMAS_Repositories.FoodRepositories;
using SMAS_Repositories.OrderRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SMAS_Services.AiBaseServices
{
    public class AIAnalysisServices : IAIAnalysisServices
    {
        private readonly ICustomerFeedbackRepository _customerFeedbackRepository;

        private readonly IFoodRepository _foodRepository;

        private readonly IOrderRepository _orderRepository;

        private readonly RestaurantDbContext _context;

        private readonly IAIService _aiService;

        public AIAnalysisServices(ICustomerFeedbackRepository customerFeedbackRepository, IAIService aiService, IOrderRepository orderRepository, IFoodRepository foodRepository, RestaurantDbContext context)
        {
            _customerFeedbackRepository = customerFeedbackRepository;
            _aiService = aiService;
            _orderRepository = orderRepository;
            _foodRepository = foodRepository;
            _context = context;

        }

        public async Task<FeedbackSummaryDTO> AnalyzeFeedbackLast3Months()
        {
            var feedbacks = await _customerFeedbackRepository.GetFeedbackToAnalysisAsync();

            var text = string.Join("\n", feedbacks.Select(f =>
                $"- ({f.Rating} sao): {f.Comment}"
            ));

            var prompt = 
$@"Bạn là AI phân tích feedback nhà hàng.

Dữ liệu feedback 3 tháng gần nhất:

{{feedbackText}}

Hãy phân tích:
1. Tỷ lệ sentiment (positive/negative/neutral)
2. Top vấn đề khách hàng gặp phải
3. Gợi ý cải thiện

Trả JSON:
{{
  ""summary"": """",
  ""sentimentStats"": {{
    ""positive"": 0,
    ""neutral"": 0,
    ""negative"": 0
  }},
  ""topIssues"": [],
  ""suggestions"": []
}}";

            var json = await _aiService.AskAI(prompt);

            var result = JsonSerializer.Deserialize<FeedbackSummaryDTO>(json);

            if (result == null)
                throw new Exception("AI trả về dữ liệu không hợp lệ");

            return result;
        }


        public async  Task<MenuAnalysisDTO> AnalyzeMenuLast3Months()
        {
            var fromDate = DateTime.Now.AddMonths(-3);

            var Orders = await _orderRepository.GetAllActiveOrderAsync();
            var orderItems = _context.OrderItems.Where(oi => oi.Order.CreatedAt >= fromDate)
                                        .Include(oi => oi.Food)
                                        .ToList();
            var feedbacks = await _customerFeedbackRepository.GetFeedbackToAnalysisAsync();
            var foods = await _foodRepository.GetAllFoodsCategoryAsync();

            var foodStats = orderItems
                .Where(o => o.FoodId != null)
                .GroupBy(o => new { o.FoodId, o.Food.Name })
                .Select(g => new
                {
                    Name = g.Key.Name,
                    Total = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            var orderedFoodNames = foodStats.Select(f => f.Name).ToHashSet();

            var notOrderedFoods = foods
                .Where(f => !orderedFoodNames.Contains(f.Name))
                .Select(f => f.Name)
                .ToList();

            var foodText = string.Join("\n", foodStats.Select(f =>
                $"- {f.Name}: {f.Total} lần"));

            var notOrderedText = string.Join("\n", notOrderedFoods.Select(f =>
                $"- {f}"));

            var prompt = $@"
Bạn là AI phân tích thực đơn nhà hàng.

Dữ liệu 3 tháng gần nhất:

Món bán:
{foodText}

Món không ai gọi:
{notOrderedText}

Hãy phân tích và đưa ra danh sách đề xuất.

Mỗi item gồm:
- type: Keep | Remove | Improve | Add
- name: tên món
- reason: lý do ngắn gọn
- detailAnalysis: phân tích chi tiết (dựa trên dữ liệu)

Trả về JSON:

{{
  ""items"": [
    {{
      ""type"": ""Keep"",
      ""name"": ""Gà rán"",
      ""reason"": ""Bán chạy"",
      ""detailAnalysis"": ""Món có tần suất gọi cao nhất trong 3 tháng gần đây""
    }}
  ]
}}

Yêu cầu:
- Không phải lúc nào cũng có đủ 4 loại
- Không thêm text ngoài JSON
";

            var json = await _aiService.AskAI(prompt);

            var result = JsonSerializer.Deserialize<MenuAnalysisDTO>(json);

            if (result == null)
                throw new Exception("AI trả dữ liệu lỗi");

            return result;
        }

        public async Task<ComboAnalysisDTO> AnalyzeComboAsync()
        {
            var fromDate = DateTime.Now.AddMonths(-3);

            var orders = await _context.Orders.Where(o => o.CreatedAt >= fromDate)
                                        .Include(o => o.OrderItems)
                                            .ThenInclude(oi => oi.Food)
                                        .ToListAsync();

            var comboStats = new Dictionary<string, int>();

            foreach (var order in orders)
            {
                var foods = order.OrderItems
                    .Where(x => x.FoodId != null)
                    .Select(x => x.Food.Name)
                    .Distinct()
                    .ToList();

                for (int i = 0; i < foods.Count; i++)
                {
                    for (int j = i + 1; j < foods.Count; j++)
                    {
                        var key = $"{foods[i]} + {foods[j]}";

                        if (!comboStats.ContainsKey(key))
                            comboStats[key] = 0;

                        comboStats[key]++;
                    }
                }
            }

            var topCombos = comboStats
                .OrderByDescending(x => x.Value)
                .Take(20)
                .Select(x => $"{x.Key}: {x.Value} lần")
                .ToList();

            var existingCombos = await _context.Combos.Include(c => c.ComboFoods).ThenInclude(cf => cf.Food).ToListAsync();

            var existingText = string.Join("\n", existingCombos.Select(c =>
                $"- {c.Name}: {string.Join(", ", c.ComboFoods.Select(f => f.Food.Name))}"
            ));

            var comboText = string.Join("\n", topCombos);

            var prompt = $@"
Bạn là AI phân tích combo nhà hàng.

Dữ liệu 3 tháng:

Các món hay gọi cùng nhau:
{comboText}

Combo hiện tại:
{existingText}

Hãy phân tích và đề xuất:

- type: Create | Improve | Remove
- comboName
- foods: danh sách món
- reason
- detailAnalysis

Trả JSON:

{{
  ""items"": [
    {{
      ""type"": ""Create"",
      ""comboName"": ""Combo Gà + Pepsi"",
      ""foods"": [""Gà rán"", ""Pepsi""],
      ""reason"": ""Thường được gọi cùng"",
      ""detailAnalysis"": ""Xuất hiện 120 lần trong 3 tháng nhưng chưa có combo""
    }}
  ]
}}

Yêu cầu:
- Không trùng combo đã có
- Không giải thích ngoài JSON
";

        
            var json = await _aiService.AskAI(prompt);

            var result = JsonSerializer.Deserialize<ComboAnalysisDTO>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });

            if (result == null || result.Items == null)
                throw new Exception("AI combo lỗi");

            return result;
        }

    }
}
