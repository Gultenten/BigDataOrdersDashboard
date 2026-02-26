//using BigDataOrdersDashboard.Context;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.ML;
//using Microsoft.ML.Runtime;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Text.Json;

//namespace BigDataOrdersDashboard.ViewComponents.CustomerDetailViewComponents
//{
//    public class _CustomerDetailAIAnalysisByLasOrdersComponentsPartial : ViewComponent
//    {
//        private readonly BigDataOrderDbContext _context;
//        private readonly IHttpClientFactory _httpClientFactory;

//        public _CustomerDetailAIAnalysisByLasOrdersComponentsPartial(BigDataOrderDbContext context, IHttpClientFactory httpClientFactory)
//        {
//            _context = context;
//            _httpClientFactory = httpClientFactory;
//        }

//        //        public async Task<IViewComponentResult> InvokeAsync(int id)
//        {
//            id = 8;


//            //müşteri listesi
//            var customer = _context.Customers
//                .Include(c => c.Orders)
//                .ThenInclude(o => o.Product)
//                .ThenInclude(p => p.Category)
//                .Where(c => c.CustomerId == id)
//                .Select(c => new
//                {
//                    c.CustomerName,
//                    c.CustomerSurname,
//                    Orders = c.Orders.OrderByDescending(o => o.OrderDate)
//                    .Take(20)
//                    .Select(o => new
//                    {
//                        o.OrderDate,
//                        Product = o.Product.ProductName,
//                        Category = o.Product.Category.CategoryName,
//                        o.Quantity,
//                        o.Product.UnitPrice,
//                        TotalPrice = o.Quantity * o.Product.UnitPrice


//                    })
//                }).FirstOrDefault();


//            var jsondata = JsonSerializer.Serialize(customer);

//            //prompt yazımı
//            string prompt = $@"
//             Sen bir veri analisti ve müşteri davranış uzmanısın.
//            Aşağıda bir müşterinin son 20 siparişine ait Json verisi bulunmaktadır.
//              Bu veriyi analiz ederek şu başlıklarla detaylı bir müşteri analiz raporu oluştur.

//1-Müşteri Profili
//2-Ürün Tercihleri
//3-Zaman Bazlı Analiz Davranışı
//4-Ortlama Harcama ve Sıklık
//5-Sadakat ve Tekrar Harcama Eğilimi
//6-Pazarlama Önerileri

//Veri:{jsondata}";

//            //OPENAI APİ İSTEĞİ
//            var httpClient = _httpClientFactory.CreateClient();
//            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "AIzaSyATYv9ama-KsaS69lEUoT54t7cqEQqICNw");//apikey gelmesi gerekiyor

//                var requestBody = new
//                {
//                    model = "gpt-4o-mini",

//                    messages = new[]
//                    {
//                        new { role = "system", content = "You are marketing data analyst." },
//                        new { role = "user", content = prompt },

//                    },
//                    temperature = 0.5

//                };

//            var jsonRequest = JsonSerializer.Serialize(requestBody);
//            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");


//            //İSTEĞİ GÖNDERME
//            var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

//            var responseString= await response.Content.ReadAsStringAsync();

//            //json cevabı ayrıştır
//            var doc=JsonDocument.Parse(responseString);

//            string completion = "AI yanıtı alınamadı.";

//            if (doc.RootElement.TryGetProperty("choices", out var choices) &&
//                choices.GetArrayLength() > 0 &&
//                choices[0].TryGetProperty("message", out var message) &&
//                message.TryGetProperty("content", out var contentElement))
//            {
//                completion = contentElement.GetString();
//            }
//            else if (doc.RootElement.TryGetProperty("error", out var error))
//            {
//                completion = "API Hatası: " + error.GetProperty("message").GetString();
//            }

//            //  var completion = doc.RootElement
//            //      .GetProperty("choices")[0]
//            //      .GetProperty("message")
//            //      .GetProperty("content")
//            //      .GetString();

//            ////  ViewBag.customer = $"{customer.CustomerName} {customer.CustomerSurname}";
//            ViewBag.Analysis = completion;


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

namespace BigDataOrdersDashboard.ViewComponents.CustomerDetailViewComponents
{
    public class _CustomerDetailAIAnalysisByLasOrdersComponentsPartial : ViewComponent
    {
        private readonly BigDataOrderDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey = "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";

        public _CustomerDetailAIAnalysisByLasOrdersComponentsPartial(BigDataOrderDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            id = 8;

            var customer = _context.Customers
                .Include(c => c.Orders)
                .ThenInclude(o => o.Product)
                .ThenInclude(p => p.Category)
                .Where(c => c.CustomerId == id)
                .Select(c => new
                {
                    c.CustomerName,
                    c.CustomerSurname,
                    Orders = c.Orders.OrderByDescending(o => o.OrderDate)
                        .Take(20)
                        .Select(o => new
                        {
                            o.OrderDate,
                            Product = o.Product.ProductName,
                            Category = o.Product.Category.CategoryName,
                            o.Quantity,
                            o.Product.UnitPrice,
                            TotalPrice = o.Quantity * o.Product.UnitPrice
                        })
                })
                .FirstOrDefault();

            var jsondata = JsonSerializer.Serialize(customer);

            string prompt = $@"
CRITICAL INSTRUCTION: Output ONLY raw HTML. 
DO NOT use markdown. 
DO NOT use code blocks. 
DO NOT write ```html or ```.
Start your response directly with <h4> tag.

⚠️ Çok önemli:
Kesinlikle ``` (backtick) veya kod bloğu verme.
Sadece saf HTML üret. Markdown verme. Kod bloğu verme.
Sen bir veri analisti ve müşteri davranış uzmanısın.
Aşağıdaki veriyi analiz et ve sonucu HTML formatında ver.
Bu başlıkları kullan (sırasını ve isimleri değiştirme):

<h4>👤 Müşteri Profili</h4>
<p><b>Ad:</b> ...</p>
<p><b>Soyad:</b> ...</p>
<p><b>Toplam Sipariş:</b> ...</p>
<p><b>Toplam Harcama:</b> ...</p>

<h4>🛍️ Ürün Tercihleri</h4>
<ul>
  <li>🏠 Ev & Dekorasyon – X sipariş</li>
  <li>💄 Kozmetik – X sipariş</li>
</ul>
<p><b>Öne çıkan ürünler:</b></p>
<ul>
  <li>Ürün adı (adet — fiyat)</li>
</ul>

<h4>⏰ Zaman Bazlı Alışveriş Davranışı</h4>
<p>En yoğun ay: ...</p>
<p>En yoğun gün: ...</p>
<p>Favori saat aralığı: ...</p>

<h4>💰 Ortalama Harcama ve Sıklık</h4>
<p>Aylık ortalama sipariş: ...</p>
<p>Ortalama sepet tutarı: ...</p>
<p>En yüksek sipariş: ...</p>
<p>En düşük sipariş: ...</p>

<h4>🎯 Sadakat ve Tekrar Harcama Eğilimi</h4>
<p>Tekrar alışveriş eğilimi: ...</p>
<p>Marka sadakati: ...</p>
<p>Kategori sadakati: ...</p>

<h4>🚀 Pazarlama Önerileri</h4>
<ul>
  <li>🎁 Kampanya önerisi: ...</li>
  <li>✉️ Hedefli e-posta: ...</li>
  <li>🆕 Yeni ürün tanıtımı önerisi: ...</li>
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

            ViewBag.RawCompletion = completion;
            return View();
        }
    }
}