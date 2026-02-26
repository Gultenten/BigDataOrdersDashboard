

//using BigDataOrdersDashboard.Context;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Text.Json;

//namespace BigDataOrdersDashboard.ViewComponents.CustomerReviewViewComponents
//{
//    public class _CustomerReviewWithOpenAIAnalysisComponentPartial : ViewComponent
//    {
//        private readonly BigDataOrderDbContext _context;
//        private readonly IHttpClientFactory _httpClientFactory;

//        public _CustomerReviewWithOpenAIAnalysisComponentPartial(BigDataOrderDbContext context, IHttpClientFactory httpClientFactory)
//        {
//            _context = context;
//            _httpClientFactory = httpClientFactory;
//        }

//        public async Task<IViewComponentResult> InvokeAsync(int id)
//        {
//            // Son 10 yorum
//            id = 657;
//            var reviews = await _context.Reviews
//                .Where(r => r.CustomerId == id)
//                .OrderByDescending(r => r.ReviewDate)
//                .Take(10)
//                .Select(r => new
//                {
//                    r.Rating,
//                    r.Sentiment,
//                    r.ReviewText,
//                    r.ReviewDate
//                })
//                .ToListAsync();

//            if (reviews == null || !reviews.Any())
//            {
//                ViewBag.AnalysisSection1 = "<h4>Veri bulunamadı</h4><p>Bu müşterinin yorumu yok.</p>";
//                return View();
//            }

//            var jsonData = JsonSerializer.Serialize(reviews);

//            string prompt = $@"
//⚠️ Sadece saf HTML üret. Kod bloğu üretme. Markdown verme.

//Sen bir müşteri davranış analisti + psikoloji destekli yorum analiz uzmanısın.

//Aşağıdaki müşteriye ait son yorumları analiz et ve HTML oluştur.

//Kullanacağın başlıklar ve format:

//<h4>👤 Müşteri Yorum Profili</h4>
//<p><b>Genel tutum:</b> ...</p>
//<p><b>Yorum tarzı:</b> ...</p>
//<p><b>Ortalama Rating:</b> ...</p>

//<h4>📊 Duygu & Ton Analizi</h4>
//<p><b>Olumlu:</b> ...</p>
//<p><b>Olumsuz:</b> ...</p>
//<p><b>Nötr:</b> ...</p>
//<p><b>Dil ve ton:</b> ...</p>

//<h4>🧠 Karakter Analizi (Review Temelli)</h4>
//<ul>
//<li>Memnuniyet eşiği: ...</li>
//<li>Şikayet hassasiyeti: ...</li>
//<li>Beklenti seviyesi: ...</li>
//<li>Kişilik tipi: ...</li>
//</ul>

//<h4>🔥 Şikayet & Övgü Temaları</h4>
//<p><b>Şikayet ettiği konular:</b></p>
//<ul><li>...</li></ul>

//<p><b>Övdüğü konular:</b></p>
//<ul><li>...</li></ul>

//<h4>📈 Davranış Trendi</h4>
//<p><b>Duygu değişimi:</b> ...</p>
//<p><b>Memnuniyet eğilim:</b> ...</p>
//<p><b>Risk analizi:</b> ...</p>

//<h4>🚀 Aksiyon & İletişim Stratejisi</h4>
//<ul>
//<li>Müşteriye yaklaşım:</li>
//<li>Destek iletişim dili:</li>
//<li>Müşteri bağlılığını artırma önerisi:</li>
//</ul>

//Veri: {jsonData}
//";

//            var httpClient = _httpClientFactory.CreateClient();
//            httpClient.DefaultRequestHeaders.Authorization =
//                new AuthenticationHeaderValue("Bearer", "YOUR_OPENAI_KEY");

//            var requestBody = new
//            {
//                model = "gpt-4o-mini",
//                messages = new[]
//                {
//                    new { role = "system", content = "You are an expert customer sentiment and behavior analyst." },
//                    new { role = "user", content = prompt }
//                },
//                temperature = 0.5
//            };

//            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
//            var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
//            var responseString = await response.Content.ReadAsStringAsync();

//            var doc = JsonDocument.Parse(responseString);
//            var completion = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

//            string[] sections = completion.Split("<h4>");

//            ViewBag.AnalysisSection1 = "<h4>" + sections.ElementAtOrDefault(1);
//            ViewBag.AnalysisSection2 = "<h4>" + sections.ElementAtOrDefault(2);
//            ViewBag.AnalysisSection3 = "<h4>" + sections.ElementAtOrDefault(3);
//            ViewBag.AnalysisSection4 = "<h4>" + sections.ElementAtOrDefault(4);
//            ViewBag.AnalysisSection5 = "<h4>" + sections.ElementAtOrDefault(5);
//            ViewBag.AnalysisSection6 = "<h4>" + sections.ElementAtOrDefault(6);

//            return View();
//        }
//    }
//}



using BigDataOrdersDashboard.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BigDataOrdersDashboard.ViewComponents.CustomerReviewViewComponents
{
    public class _CustomerReviewWithOpenAIAnalysisComponentPartial : ViewComponent
    {
        private readonly BigDataOrderDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey = "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";

        public _CustomerReviewWithOpenAIAnalysisComponentPartial(BigDataOrderDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            id = 657;

            var reviews = await _context.Reviews
                .Where(r => r.CustomerId == id)
                .OrderByDescending(r => r.ReviewDate)
                .Take(10)
                .Select(r => new
                {
                    r.Rating,
                    r.Sentiment,
                    r.ReviewText,
                    r.ReviewDate
                })
                .ToListAsync();

            if (reviews == null || !reviews.Any())
            {
                ViewBag.AnalysisSection1 = "<p>Bu müşterinin yorumu yok.</p>";
                return View();
            }

            var jsondata = JsonSerializer.Serialize(reviews);

            string prompt = $@"
CRITICAL INSTRUCTION: Output ONLY raw HTML. 
DO NOT use markdown. 
DO NOT use code blocks. 
DO NOT write ```html or ```.
Start your response directly with <h4> tag.

⚠️ Sadece saf HTML üret. Kod bloğu üretme. Markdown verme.
Sen bir müşteri davranış analisti + psikoloji destekli yorum analiz uzmanısın.
Aşağıdaki müşteriye ait son yorumları analiz et ve HTML oluştur.

<h4>👤 Müşteri Yorum Profili</h4>
<p><b>Genel tutum:</b> ...</p>
<p><b>Yorum tarzı:</b> ...</p>
<p><b>Ortalama Rating:</b> ...</p>

<h4>📊 Duygu & Ton Analizi</h4>
<p><b>Olumlu:</b> ...</p>
<p><b>Olumsuz:</b> ...</p>
<p><b>Nötr:</b> ...</p>
<p><b>Dil ve ton:</b> ...</p>

<h4>🧠 Karakter Analizi (Review Temelli)</h4>
<ul>
<li>Memnuniyet eşiği: ...</li>
<li>Şikayet hassasiyeti: ...</li>
<li>Beklenti seviyesi: ...</li>
<li>Kişilik tipi: ...</li>
</ul>

<h4>🔥 Şikayet & Övgü Temaları</h4>
<p><b>Şikayet ettiği konular:</b></p>
<ul><li>...</li></ul>
<p><b>Övdüğü konular:</b></p>
<ul><li>...</li></ul>

<h4>📈 Davranış Trendi</h4>
<p><b>Duygu değişimi:</b> ...</p>
<p><b>Memnuniyet eğilim:</b> ...</p>
<p><b>Risk analizi:</b> ...</p>

<h4>🚀 Aksiyon & İletişim Stratejisi</h4>
<ul>
<li>Müşteriye yaklaşım: ...</li>
<li>Destek iletişim dili: ...</li>
<li>Müşteri bağlılığını artırma önerisi: ...</li>
</ul>

Veri: {jsondata}";

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);

            var requestBody = new
            {
                model = "google/gemma-3-4b-it:free",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                temperature = 0.5
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(
                "https://openrouter.ai/api/v1/chat/completions",
                content);

            var responseString = await response.Content.ReadAsStringAsync();
            string completion = "AI yanıtı alınamadı.";

            try
            {
                using var doc = JsonDocument.Parse(responseString);

                if (doc.RootElement.TryGetProperty("choices", out var choices) &&
                    choices.GetArrayLength() > 0 &&
                    choices[0].TryGetProperty("message", out var message) &&
                    message.TryGetProperty("content", out var textElement))
                {
                    completion = textElement.GetString();
                    completion = completion
       .Replace("```html", "")
       .Replace("```", "")
       .Trim();
                }
                else if (doc.RootElement.TryGetProperty("error", out var error))
                {
                    completion = "API Hatası: " + error.GetProperty("message").GetString();
                }
                else
                {
                    completion = "Beklenmeyen yanıt: " + responseString;
                }
            }
            catch (Exception ex)
            {
                completion = $"JSON parse hatası: {ex.Message} | Response: {responseString}";
            }

            ViewBag.RawCompletion = completion;

            string[] sections = completion.Split("<h4>");

            string CleanSection(string s)
            {
                if (string.IsNullOrEmpty(s)) return "";
                var closeTag = s.IndexOf("</h4>");
                if (closeTag >= 0)
                    s = s.Substring(closeTag + 5);
                return s.Trim();
            }

            ViewBag.AnalysisSection1 = sections.Length > 1 ? CleanSection(sections[1]) : "";
            ViewBag.AnalysisSection2 = sections.Length > 2 ? CleanSection(sections[2]) : "";
            ViewBag.AnalysisSection3 = sections.Length > 3 ? CleanSection(sections[3]) : "";
            ViewBag.AnalysisSection4 = sections.Length > 4 ? CleanSection(sections[4]) : "";
            ViewBag.AnalysisSection5 = sections.Length > 5 ? CleanSection(sections[5]) : "";
            ViewBag.AnalysisSection6 = sections.Length > 6 ? CleanSection(sections[6]) : "";

            return View();
        }
    }
}