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
}}

Yêu cầu:
- nếu dữ liệu không đủ để phân tích, trả về  rỗng

";


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
  ""Items"": [
    {{
      ""type"": ""Keep"",
      ""name"": ""Gà rán"",
      ""reason"": ""Bán chạy"",
      ""detailAnalysis"": ""Món có tần suất gọi cao nhất trong 3 tháng gần đây""
    }}
  ]
}}

Yêu cầu:
- nếu dữ liệu không đủ để phân tích, trả về Items rỗng
- Key phải đúng EXACT: Items, Type, Name, Reason, DetailAnalysis
- Không dùng chữ thường
- Không trả null
- Không thêm text ngoài JSON
- chỉ trả về không quá 10 items
";

            var json = await _aiService.AskAI(prompt);

            var result = JsonSerializer.Deserialize<MenuAnalysisDTO>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result.Items == null)
                throw new Exception("AI trả dữ liệu lỗi");

            return result;
        }






        public async Task<ComboAnalysisDTO> AnalyzeComboAsync()
        {
            var fromDate = DateTime.Now.AddMonths(-3);

            // 1. Lấy order + món
            var orders = await _context.Orders
                .Where(o => o.CreatedAt >= fromDate)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Food)
                .ToListAsync();

            // 2. Lấy MENU hiện tại
            var foods = await _context.Foods
                .Where(f => f.IsAvailable == true || f.IsAvailable == null)
                .Select(f => f.Name)
                .ToListAsync();

            var menuText = string.Join("\n", foods.Select(f => $"- {f}"));

            // 3. Lấy combo từ đơn hàng (group nhiều món, không chỉ pair)
            var orderCombos = orders
                .Select(o => o.OrderItems
                    .Where(x => x.FoodId != null && x.Food != null)
                    .Select(x => x.Food.Name)
                    .Distinct()
                    .ToList())
                .Where(list => list.Count >= 2)
                .ToList();

            var comboStats = orderCombos
                .GroupBy(c => string.Join(", ", c.OrderBy(x => x)))
                .Select(g => new
                {
                    Combo = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(20)
                .ToList();

            var comboText = string.Join("\n", comboStats.Select(c =>
                $"- {c.Combo}: {c.Count} lần"));

            // 4. Combo hiện tại
            var existingCombos = await _context.Combos
                .Include(c => c.ComboFoods)
                    .ThenInclude(cf => cf.Food)
                .ToListAsync();

            var existingText = string.Join("\n", existingCombos.Select(c =>
                $"- {c.Name}: {string.Join(", ", c.ComboFoods.Select(f => f.Food.Name))}"
            ));

            // 5. PROMPT (đã fix chuẩn)
            var prompt = $@"
Bạn là AI phân tích combo nhà hàng.

Nhà hàng chuyên: LẨU & NƯỚNG

===== MENU HIỆN TẠI =====
{menuText}

===== DỮ LIỆU 3 THÁNG =====
Các nhóm món khách thường gọi chung:
{comboText}

===== COMBO HIỆN TẠI =====
{existingText}

===== YÊU CẦU =====
Hãy phân tích và đề xuất:

- type: Create | Improve | Remove
- comboName
- foods: danh sách món (PHẢI có trong MENU)
- reason
- detailAnalysis

===== FORMAT JSON (BẮT BUỘC) =====
{{
  ""Items"": [
    {{
      ""Type"": ""Create"",
      ""ComboName"": ""Combo Lẩu Hải Sản"",
      ""Foods"": [""Lẩu Thái"", ""Tôm Sú""],
      ""Reason"": ""Hay được gọi cùng"",
      ""DetailAnalysis"": ""Xuất hiện nhiều lần trong đơn hàng""
    }}
  ]
}}

===== RULE =====
- Chỉ dùng món có trong MENU
- Ưu tiên combo lẩu/nướng
- Combo phải có 2 → 10 món
- Không trùng combo đã có
- Không trả null
- Không thêm text ngoài JSON
- Tối đa 5 items
";

            // 6. Gọi AI
            var json = await _aiService.AskAI(prompt);

            Console.WriteLine("===== CLEAN JSON =====");
            Console.WriteLine(json);

            // 7. Deserialize chuẩn
            var result = JsonSerializer.Deserialize<ComboAnalysisDTO>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result == null || result.Items == null || !result.Items.Any())
                throw new Exception("AI combo lỗi hoặc không có dữ liệu");

            return result;
        }

    }
}
