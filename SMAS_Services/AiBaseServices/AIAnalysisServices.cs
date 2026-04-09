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

            var fromDate = DateTime.Now.AddMonths(-3);

            var Orders = await _orderRepository.GetAllActiveOrderAsync();
            var orderItems = _context.OrderItems.Where(oi => oi.Order.CreatedAt >= fromDate)
                                        .Include(oi => oi.Food)
                                        .ToList();
            var feedbacks = await _customerFeedbackRepository.GetFeedbackToAnalysisAsync();
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

            // 🔥 Lấy top món phổ biến (context cho AI)
            var topFoods = foodStats
                .Take(15)
                .Select((f, index) => new
                {
                    f.Name,
                    f.Total,
                    Rank = index + 1
                })
                .ToList();

            var foodContextText = string.Join("\n", topFoods.Select(f =>
                $"- {f.Name}: {f.Total} lần | Rank {f.Rank}"
            ));

            // 🔥 NÂNG CẤP: Gửi kèm món trong đơn
            var feedbackText = string.Join("\n", feedbacks.Select(f =>
                $"- ({f.Rating} sao): {f.Comment}"
            ));

            var prompt = $@"
Bạn là chuyên gia phân tích trải nghiệm khách hàng (Customer Experience Analyst).

===== FEEDBACK KHÁCH HÀNG =====
{feedbackText}

===== NGỮ CẢNH MÓN ĂN (TOP MÓN BÁN CHẠY) =====
{foodContextText}

===== LƯU Ý =====
- Feedback là theo đơn, không phải từng món
- Danh sách món chỉ dùng làm NGỮ CẢNH
- KHÔNG được khẳng định chắc chắn món bị lỗi nếu không có bằng chứng rõ

===== NHIỆM VỤ =====

1. SUMMARY
- Tổng quan trải nghiệm khách hàng
- Chỉ ra điểm mạnh & điểm yếu chính

2. SENTIMENT (%)
- PositivePercent
- NeutralPercent
- NegativePercent

3. TOP ISSUES

Phân loại:
- Food
- Service
- Price
- Environment

Mỗi issue gồm:
- IssueName
- Category
- Percent
- Severity (Low | Medium | High)
- Description (insight rõ ràng, không chung chung)

4. SUGGESTIONS

- Title
- Detail (hành động cụ thể)
- Priority (Low | Medium | High)

===== FORMAT JSON =====
{{
  ""Summary"": ""..."",
  ""SentimentStats"": {{
    ""PositivePercent"": 0,
    ""NeutralPercent"": 0,
    ""NegativePercent"": 0
  }},
  ""TopIssues"": [
    {{
      ""IssueName"": ""..."",
      ""Category"": ""..."",
      ""Percent"": 0,
      ""Severity"": ""..."",
      ""Description"": ""...""
    }}
  ],
  ""Suggestions"": [
    {{
      ""Title"": ""..."",
      ""Detail"": ""..."",
      ""Priority"": ""...""
    }}
  ]
}}

===== RULE =====
- Không trả text ngoài JSON
- Không null
- Phải có % nếu có thể
- Insight không lặp
- Không suy diễn sai món ăn
- Nếu dữ liệu ít → trả rỗng
";


            var json = await _aiService.AskAI(prompt);

            var result = JsonSerializer.Deserialize<FeedbackSummaryDTO>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result == null)
                throw new Exception("AI trả về dữ liệu không hợp lệ");

            return result;
        }


        public async Task<MenuAnalysisDTO> AnalyzeMenuLast3Months()
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

            // ===== 🔥 NÂNG CẤP DATA (THÊM % + RANK) =====
            var totalOrders = foodStats.Sum(f => f.Total);

            var foodStatsEnhanced = foodStats
                .Select((f, index) => new
                {
                    f.Name,
                    f.Total,
                    Percent = totalOrders > 0 ? Math.Round((double)f.Total / totalOrders * 100, 2) : 0,
                    Rank = index + 1
                })
                .ToList();

            var orderedFoodNames = foodStatsEnhanced.Select(f => f.Name).ToHashSet();

            var notOrderedFoods = foods
                .Where(f => !orderedFoodNames.Contains(f.Name))
                .Select(f => f.Name)
                .ToList();

            // ===== BUILD TEXT =====
            var foodText = string.Join("\n", foodStatsEnhanced.Select(f =>
                $"- {f.Name}: {f.Total} lần | {f.Percent}% | Rank {f.Rank}"));

            var notOrderedText = string.Join("\n", notOrderedFoods.Select(f =>
                $"- {f}"));

            var feedbackText = string.Join("\n", feedbacks.Take(5).Select(f =>
                $"- {f.Comment}"));

            // ===== 🔥 PROMPT NÂNG CẤP =====
            var prompt = $@"
Bạn là chuyên gia tư vấn chiến lược nhà hàng.

Dữ liệu 3 tháng gần nhất:

Mỗi món gồm:
- Total: số lượt gọi
- Percent (%): tỷ trọng trong toàn bộ menu
- Rank: thứ hạng theo mức độ phổ biến

Danh sách món:
{foodText}

Món không ai gọi:
{notOrderedText}

Feedback khách hàng:
{feedbackText}

Nhiệm vụ:
Đánh giá thực đơn dưới góc nhìn kinh doanh và hành vi khách hàng.

Không chỉ đọc số liệu, mà phải:
- Hiểu vai trò của món (món chủ lực, bổ trợ, niche)
- Nhìn vào sự phân bổ menu (có bị lệch không)
- Phát hiện cơ hội hoặc rủi ro (ví dụ: phụ thuộc 1 món)

Hướng phân tích:

1. Với món top (Percent cao, Rank thấp):
- Không chỉ nói “bán chạy”
- Phải phân tích vì sao (ví dụ: dễ ăn, phổ biến, giá hợp lý…)
- Nếu Percent > 30% → cảnh báo phụ thuộc

2. Với món trung bình:
- Xem có tiềm năng phát triển hay đang bị lép vế
- So sánh với món khác

3. Với món không có lượt gọi:
- Không chỉ nói “không ai gọi”
- Phải suy luận nguyên nhân:
  + không hợp thị hiếu
  + tên món không hấp dẫn
  + không phù hợp concept menu

4. Phát hiện vấn đề tổng thể:
- Menu có bị phụ thuộc vào 1–2 món không
- Có thiếu đa dạng không
- Có nhóm món nào yếu không

Trả về JSON:

{{
  ""Items"": [
    {{
      ""Type"": ""..."",
      ""Level"": ""..."",
      ""Name"": ""..."",
      ""Reason"": ""..."",
      ""DetailAnalysis"": ""...""
    }}
  ]
}}

Yêu cầu:
- Level có thể là: TOP | MID | LOW | NORMAL
- Mỗi item phải có insight KHÁC NHAU
- Không lặp lại cách giải thích
- Ưu tiên phân tích ""tại sao"" hơn ""bao nhiêu""
- Khi cần, sử dụng Percent (%) để làm rõ mức độ
- Viết như đang tư vấn cho chủ nhà hàng
- Không thêm text ngoài JSON
- Không quá 10 items
- Tất cả trả về tiếng việt trừ level
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

            // 3. Lấy combo từ đơn hàng
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

            // ===== 🔥 NÂNG CẤP DATA (THÊM % + RANK) =====
            var totalCombos = orderCombos.Count;

            var comboStatsEnhanced = comboStats
                .Select((c, index) => new
                {
                    c.Combo,
                    c.Count,
                    Percent = totalCombos > 0 ? Math.Round((double)c.Count / totalCombos * 100, 2) : 0,
                    Rank = index + 1
                })
                .ToList();

            var comboText = string.Join("\n", comboStatsEnhanced.Select(c =>
                $"- {c.Combo}: {c.Count} lần | {c.Percent}% | Rank {c.Rank}"));

            // 4. Combo hiện tại
            var existingCombos = await _context.Combos
                .Include(c => c.ComboFoods)
                    .ThenInclude(cf => cf.Food)
                .ToListAsync();

            var existingText = string.Join("\n", existingCombos.Select(c =>
                $"- {c.Name}: {string.Join(", ", c.ComboFoods.Select(f => f.Food.Name))}"
            ));

            // ===== 🔥 PROMPT NÂNG CẤP (CÓ PATTERN NHƯNG KHÔNG ĐỔI DTO) =====
            var prompt = $@"
Bạn là chuyên gia xây dựng combo cho nhà hàng.

Nhà hàng chuyên: LẨU & NƯỚNG

===== MENU HIỆN TẠI =====
{menuText}

===== DỮ LIỆU HÀNH VI KHÁCH HÀNG (3 THÁNG) =====
Các nhóm món khách thường gọi cùng:

Mỗi combo gồm:
- Count: số lần xuất hiện
- Percent (%): tỷ lệ xuất hiện
- Rank: mức độ phổ biến

{comboText}

===== COMBO HIỆN TẠI =====
{existingText}

===== NHIỆM VỤ =====
Phân tích hành vi khách hàng và đề xuất combo mang tính KINH DOANH.

===== PHÂN TÍCH PATTERN (QUAN TRỌNG) =====
Trước khi đề xuất, hãy NGẦM hiểu các pattern hành vi:

- Lẩu + topping → combo ăn chính
- Nướng + nước/bia → combo nhóm
- 2–3 món → combo tiết kiệm / ăn nhẹ
- Nhiều món (>=5) → khách đi nhóm đông

KHÔNG cần trả ra pattern riêng,
nhưng BẮT BUỘC phải phản ánh pattern này trong DetailAnalysis.

===== HƯỚNG PHÂN TÍCH =====

1. Combo phổ biến (Percent cao):
- Không chỉ nói ""hay gọi cùng""
- Phải phân tích theo pattern (ăn chính, nhóm, ăn nhẹ…)

2. Combo trung bình:
- Xem có thể tối ưu thành combo bán được không

3. Combo yếu:
- Không nên đề xuất nếu không có logic rõ

4. Combo hiện tại:
- Nếu không khớp hành vi khách → Improve hoặc Remove

===== OUTPUT =====

- Type: Create | Improve | Remove
- ComboName
- Foods (PHẢI thuộc MENU)
- Reason
- DetailAnalysis (PHẢI có phân tích pattern)

===== FORMAT JSON =====
{{
  ""Items"": [
    {{
      ""Type"": ""Create"",
      ""ComboName"": ""Combo Lẩu Thái No Nê Nhóm 3"",
      ""Foods"": [""Lẩu Thái"", ""Bò Mỹ"", ""Nấm""],
      ""OriginalPrice"": 350000,
      ""SuggestedPrice"": 299000,
      ""DiscountPercent"": 15,
      ""Reason"": ""Phù hợp khách đi nhóm"",
      ""DetailAnalysis"": ""...""
    }}
  ]
}}

===== RULE =====
- Chỉ dùng món trong MENU
- Ưu tiên combo lẩu/nướng
- Combo phải có 2–10 món
- Không trùng combo hiện tại
- Insight phải KHÁC NHAU
- Phải dùng Percent hoặc Rank khi hợp lý
- DetailAnalysis phải có yếu tố hành vi (pattern)
- Không thêm text ngoài JSON
- Tối đa 5 items

===== ĐẶT TÊN COMBO (MARKETING) =====

Tên combo KHÔNG được đặt chung chung.

Phải:
- Nghe hấp dẫn, dễ bán
- Gợi cảm xúc hoặc trải nghiệm
- Có thể gợi đối tượng khách (nhóm, cặp đôi…)

Ví dụ tên tốt:
- ""Combo Lẩu Thái No Nê Nhóm 3""
- ""Combo Nướng Chill Hội Bạn""
- ""Combo Ăn Nhẹ Tiết Kiệm""
- ""Combo Lẩu Hải Sản Đã Đời""

Tránh:
- ""Combo 1""
- ""Combo Lẩu""
- ""Combo A""

Yêu cầu:
- Tên phải tự nhiên như menu thật
- Không quá dài
- Không trùng nhau


===== CHIẾN LƯỢC GIÁ =====

Mỗi món trong MENU có giá.

Yêu cầu:
- Tính tổng giá gốc (OriginalPrice) = tổng giá các món
- Đề xuất giá combo (SuggestedPrice)

Quy tắc:
- Combo 2–3 món → giảm 5–10%
- Combo 4–6 món → giảm 10–20%
- Combo lớn → giảm 15–25%

- Không giảm quá sâu (tránh lỗ)
- Giá phải hợp lý, dễ bán

Trả thêm:
- OriginalPrice
- SuggestedPrice
- DiscountPercent
";

            // 6. Gọi AI
            var json = await _aiService.AskAI(prompt);

            Console.WriteLine("===== CLEAN JSON =====");
            Console.WriteLine(json);

            // 7. Deserialize
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
