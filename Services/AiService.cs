using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System;
using Dto;
using Microsoft.Extensions.Configuration;

namespace Services
{
    public class AiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private static readonly Dictionary<string, List<object>> _conversationHistory = new();
        
        private readonly string _groqApiKey;
        private readonly string _groqApiUrl;
        
        private const string SystemPrompt = @"
את נציגת שירות מקצועית של COZYGEN - חנות רהיטים ועיצוב פנים.

תפקידך:
- לעזור ללקוחות למצוא את הרהיטים המושלמים לביתם
- לתת עצות מקצועיות לעיצוב פנים
- לענות על שאלות על מוצרים, מחירים, משלוחים

מידע על החנות:
- שעות פתיחה: א-ה 9:00-20:00, ו-ש 9:00-14:00
- משלוח חינם לכל הארץ על קניות מעל 2000₪
- אחריות 12 חודשים על כל המוצרים
- אפשרות תשלום בתשלומים עד 12 תשלומים
- החזרות בתוך 14 יום

סגנונות עיצוב:
- מודרני: קווים נקיים, צבעים ניטרליים, מינימליזם
- סקנדינבי: עץ בהיר, פונקציונליות, פשטות
- קלאסי: עץ כהה, פרטים מעוטרים, אלגנטיות
- בוהו: צבעים חמים, טקסטילים, אקלקטיות

כללי עיצוב תשובות:
1. השתמשי בפסקאות קצרות ונפרדות
2. השתמשי ברשימות עם נקודות או מספרים
3. הדגישי מידע חשוב עם רווחים
4. פרקי תשובות ארוכות לחלקים
5. השתמשי באמוג'י רלוונטיים במידה

כללים:
1. תמיד עני בעברית בצורה ידידותית ומקצועית
2. התאימי את התשובות לצרכי הלקוח
3. אם את לא יודעת משהו, אמרי זאת בכנות
4. שמרי על הקשר הרציף בשיחה
5. הצעי פתרונות מעשיים
6. עצבי את התשובות בצורה נוחה לקריאה
";

        public AiService(HttpClient httpClient, IProductService productService, IOrderService orderService, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _productService = productService;
            _orderService = orderService;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            
            _groqApiKey = _configuration["GROQ_API_KEY"] ?? throw new InvalidOperationException("GROQ_API_KEY not found in configuration");
            _groqApiUrl = _configuration["GROQ_API_URL"] ?? "https://api.groq.com/openai/v1/chat/completions";
            
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_groqApiKey}");
        }

        public async Task<DtoChatResponse> ChatAsync(string userMessage)
        {
            if (string.IsNullOrWhiteSpace(userMessage))
                return new DtoChatResponse { Response = "שלום! אני כאן לעזור לך למצוא את הרהיטים המושלמים לביתך. במה אוכל לסייע?", Category = "כללי" };

            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "אורח";

            var productsResult = await _productService.GetProducts(0, 20, null, null, null, null, null);
            var productInfo = string.Join("\n", productsResult.Products.Take(20).Select(p => 
                $"- {p.Name}: {p.Price}₪"));
            
            var userOrdersInfo = "";
            if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int userIdInt))
            {
                var orders = await _orderService.GetOrdersUser(userIdInt);
                if (orders != null && orders.Any())
                {
                    userOrdersInfo = "\n\nהזמנות של הלקוח:\n" + 
                        string.Join("\n", orders.Where(o => o != null).Take(5).Select(o => 
                            $"- הזמנה #{o!.OrderId}: {o.TotalPrice}₪, תאריך: {o.OrderDate:dd/MM/yyyy}"));
                }
            }
            
            var enhancedPrompt = SystemPrompt + $"\n\nשם הלקוח: {userName}\n\nמוצרים זמינים:\n{productInfo}{userOrdersInfo}";

            var sessionId = userId ?? "guest";
            if (!_conversationHistory.ContainsKey(sessionId))
            {
                _conversationHistory[sessionId] = new List<object>
                {
                    new { role = "system", content = enhancedPrompt }
                };
            }

            _conversationHistory[sessionId].Add(new { role = "user", content = userMessage });

            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = _conversationHistory[sessionId].ToArray()
            };

            try 
            {
                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_groqApiUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return new DtoChatResponse { Response = $"סליחה, יש לי תקלה זמנית. נסה שוב בעוד רגע.", Category = "שגיאה" };

                using var doc = JsonDocument.Parse(responseString);
                var answer = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                
                _conversationHistory[sessionId].Add(new { role = "assistant", content = answer });
                
                if (_conversationHistory[sessionId].Count > 21)
                {
                    _conversationHistory[sessionId].RemoveRange(1, 2);
                }
                
                return new DtoChatResponse { Response = answer ?? "", Category = "כללי" };
            }
            catch
            {
                return new DtoChatResponse { Response = "סליחה, יש לי תקלה טכנית. אנא נסה שוב.", Category = "שגיאה" };
            }
        }

        public async Task<List<int>> AnalyzeImageAsync(
            IFormFile image,
            IEnumerable<DtoProduct_Id_Name_Category_Price_Desc_Image> allProducts,
            IEnumerable<DtoSyle_id_name> styles,
            IEnumerable<DtoCategory_Name_Id> categories)
        {
            if (image == null) return new List<int>();

     
            var stylesContext = string.Join(", ", styles.Select(s => $"{{id:{s.StyleId}, name:'{s.Name}'}}"));
            var categoriesContext = string.Join(", ", categories.Select(c => $"{{id:{c.CategoryId}, name:'{c.Name}'}}"));

            var productListContext = string.Join(", ", allProducts.Select(p => {
                var sIds = p.ProductStyles != null
                           ? string.Join(",", p.ProductStyles.Select(ps => ps.StyleId))
                           : "";

                return $"{{id:{p.ProductId}, name:'{p.Name}', desc:'{p.Description}', catId:{p.CategoryId}, styleIds:[{sIds}]}}";
            }));

          
            using var ms = new MemoryStream();
            await image.CopyToAsync(ms);
            var base64Image = Convert.ToBase64String(ms.ToArray());

            
            var prompt = $@"
                Act as an Interior Design Expert. 
                Styles: [{stylesContext}]
                Categories: [{categoriesContext}]
                Catalog: [{productListContext}]

                Task: Match the room in the image to a Style ID, then select 10-15 products with that Style ID. 
                Pay special attention to returning products whose color matches the uploaded image. 
                The style should be determined from the overall look and feel of the room.
                Also note that the color is written in the product description.
                Ensure variety across different 'categoryId' values.
                If the image is not a room image return empty array.
                Return ONLY JSON: {{ ""productIds"": [1, 2, 3] }}";

          
            var requestBody = new
            {
                model = "meta-llama/llama-4-scout-17b-16e-instruct",
                messages = new[] {
                    new {
                        role = "user",
                        content = new object[] {
                            new { type = "text", text = prompt },
                            new { type = "image_url", image_url = new { url = $"data:image/jpeg;base64,{base64Image}" } }
                        }
                    }
                },
                temperature = 0.8,
                response_format = new { type = "json_object" }
            };

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _groqApiKey);
            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_groqApiUrl, content);

            if (!response.IsSuccessStatusCode) return new List<int>();

            var responseString = await response.Content.ReadAsStringAsync();
            return ParseIdsFromResponse(responseString);
        }

        private List<int> ParseIdsFromResponse(string jsonResponse)
        {
            try
            {
                using var doc = JsonDocument.Parse(jsonResponse);
                var content = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                using var contentDoc = JsonDocument.Parse(content);
                return JsonSerializer.Deserialize<List<int>>(contentDoc.RootElement.GetProperty("productIds").GetRawText()) ?? new List<int>();
            }
            catch { return new List<int>(); }
        }
    }
}