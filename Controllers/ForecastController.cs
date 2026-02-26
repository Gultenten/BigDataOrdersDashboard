using BigDataOrdersDashboard.Context;
using BigDataOrdersDashboard.Dtos.ForecastDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using System.IO.Pipes;

namespace BigDataOrdersDashboard.Controllers
{
    public class ForecastController : Controller
    {
        private readonly BigDataOrderDbContext _context;
        private readonly MLContext _mlContext;

        public ForecastController(BigDataOrderDbContext context, MLContext mlContext)
        {
            _context = context;
            _mlContext = mlContext;
        }

        //public IActionResult PaymentMethodForecast()
        //{
        //    //2025 yılının verilerinin çekilmesi
        //    var startDate = new DateTime(2025, 1, 1);
        //    var endDate = new DateTime(2025, 12, 31);

        //    var monthlyPaymentData = _context.Orders.Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate).
        //        AsEnumerable().
        //        GroupBy(o => new
        //        {
        //            Month = new DateTime(o.OrderDate.Year, o.OrderDate.Month, 1),
        //            o.PaymentMethod

        //        }).
        //        Select(g => new
        //        {
        //            Month = g.Key.Month,
        //            PaymentMethod = g.Key.PaymentMethod,
        //            OrderCount = g.Count()

        //        })
        //        .OrderBy(x => x.Month).ToList();

        //    //Tahmin Sonuçlarını Tutacak Listes

        //    var forecasts = new List<Object>();

        //    //her ödeme yöntemi için ayrı ayrı model oluşturylması
        //    foreach (var method in monthlyPaymentData.Select(x => x.PaymentMethod).Distinct())
        //    {


        //        var methodData = monthlyPaymentData.Where(x => x.PaymentMethod == method)
        //            .Select((x, index) => new PaymentForecastData
        //            {
        //                PaymentMethod = method,
        //                MonthIndex = index + 1,
        //                OrderCount = x.OrderCount

        //            }).ToList();


        //        var dataView = _mlContext.Data.LoadFromEnumerable(methodData);

        //        //Forecast Modeli
        //        var pipeline = _mlContext.Forecasting.ForecastBySsa(
        //           outputColumnName: "ForecastedValues",
        //           inputColumnName: nameof(PaymentForecastData.OrderCount),
        //             windowSize: 12,

        //             seriesLength: methodData.Count,
        //             trainSize: methodData.Count,
        //             horizon: 12,
        //             confidenceLevel: 0.95f
        //             );

        //        var model = pipeline.Fit(dataView);
        //        var engine = model.CreateTimeSeriesEngine<PaymentForecastData, PaymentForecastPrediction>(_mlContext);
        //        var prediction = engine.Predict();


        //        //2026 yılı ocak subat mart ayı tahmnileri
        //        for (int i = 0; i < prediction.ForecastedValues.Length; i++)
        //        {
        //            forecasts.Add(new
        //            {
        //                PaymentMethod = method,
        //                Month = new DateTime(2026, i + 1, 1).ToString("yyyy MMMM"),
        //                ForeCastCount = (int)prediction.ForecastedValues[i]

        //            });


        //        }

        //    }
        //    return View(forecasts);

        //}

        //public IActionResult GermanyCitiesForecast()
        //{
        //    var startDate = new DateTime(2024, 1, 1);
        //    var endDate = new DateTime(2024, 12, 31);

        //    var germanycityData = _context.Orders
        //        .Include(o => o.Customer)
        //        .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.Customer.CustomerCountry =="Almanya")
        //        .AsEnumerable()
        //        .GroupBy(o => new
        //        {
        //            o.Customer.CustomerCity,
        //            Year = o.OrderDate.Year,
        //            Month = o.OrderDate.Month,
        //        }).Select(g => new
        //        {
        //            City = g.Key.CustomerCity,
        //            DateKey = $"{g.Key.Year}-{g.Key.Month:D2}",
        //            OrderCount = g.Count()

        //        }).OrderBy(x => x.City)
        //        .ThenBy(y => y.DateKey)
        //        .ToList();

        //    var forecasts = new List<object>();
        //    foreach (var city in germanycityData.Select(x => x.City).Distinct())
        //    {
        //        var cityData = germanycityData.Where(x => x.City == city)
        //            .Select((x, index) => new GermanyCitiesForecastData
        //            {
        //                City = city,
        //                MonthIndex = index + 1,
        //                OrderCount = x.OrderCount

        //            }).ToList();

        //        if (cityData.Count < 4)
        //        {
        //            continue;

        //        }

        //        var dataView = _mlContext.Data.LoadFromEnumerable(cityData);
        //        var pipeLine = _mlContext.Forecasting.ForecastBySsa(
        //            outputColumnName: "ForecastedValues",
        //            inputColumnName: nameof(GermanyCitiesForecastData.OrderCount),
        //            windowSize: 2,
        //            seriesLength: cityData.Count,
        //            trainSize: cityData.Count,
        //            horizon: 1,
        //            confidenceLevel: 0.95f
        //            );

        //        var model = pipeLine.Fit(dataView);
        //        var engine = model.CreateTimeSeriesEngine<GermanyCitiesForecastData, GermanyCitiesForecastPrediction>(_mlContext);

        //        var prediction = engine.Predict();
        //        forecasts.Add(new
        //        {
        //            City = city,
        //            Year = "2026",
        //            ForecastedCount = (int)prediction.ForecastedValues[0]
        //        });
        //    }
        //    return View(forecasts);
        //}







        public IActionResult GermanyCitiesForecast()
        {
            var startDate = new DateTime(2023, 1, 1);
            var endDate = new DateTime(2025, 12, 31);

            var germanyCityData = _context.Orders
                .Include(o => o.Customer)
                .Where(o => o.OrderDate >= startDate &&
                            o.OrderDate <= endDate &&
                            o.Customer.CustomerCountry == "Almanya")
                .AsEnumerable()
                .GroupBy(o => new
                {
                    o.Customer.CustomerCity,
                    Year = o.OrderDate.Year,
                    Month = o.OrderDate.Month
                })
                .Select(g => new
                {
                    City = g.Key.CustomerCity,
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    DateKey = $"{g.Key.Year}-{g.Key.Month:D2}",
                    OrderCount = g.Count()
                })
                .OrderBy(x => x.City)
                .ThenBy(x => x.DateKey)
                .ToList();

            var forecasts = new List<object>();

            foreach (var city in germanyCityData.Select(x => x.City).Distinct())
            {
                var cityData = germanyCityData
                    .Where(x => x.City == city)
                    .Select((x, index) => new GermanyCitiesForecastData
                    {
                        City = city,
                        MonthIndex = index + 1,
                        OrderCount = x.OrderCount
                    })
                    .ToList();

                // 🔥 SSA için minimum veri kontrolü
                if (cityData.Count < 6)
                    continue;

                var dataView = _mlContext.Data.LoadFromEnumerable(cityData);

                // 🔥 Dinamik windowSize (Hata çözümü burada)
                var windowSize = cityData.Count / 3;

                if (windowSize < 2)
                    continue;

                var pipeline = _mlContext.Forecasting.ForecastBySsa(
                    outputColumnName: "ForecastedValues",
                    inputColumnName: nameof(GermanyCitiesForecastData.OrderCount),
                    windowSize: windowSize,

                    seriesLength: cityData.Count,
                    trainSize: cityData.Count,
                    horizon: 12,
                    confidenceLevel: 0.95f
                );

                var model = pipeline.Fit(dataView);
                var engine = model.CreateTimeSeriesEngine
                    <GermanyCitiesForecastData, GermanyCitiesForecastPrediction>(_mlContext);

                var prediction = engine.Predict();

                //

                var yearlyForecast = (int)prediction.ForecastedValues.Sum();

                var year2024Count = germanyCityData
                    .Where(x => x.City == city && x.Year == 2024)
                    .Sum(x => x.OrderCount);

                var year2025Count = germanyCityData
                    .Where(x => x.City == city && x.Year == 2025)
                    .Sum(x => x.OrderCount);

                var diff = yearlyForecast - year2025Count;

                double? growthRate = year2025Count > 0
                    ? (diff / (double)year2025Count) * 100.0
                    : (double?)null;

                forecasts.Add(new
                {
                    City = city,
                    Year2024 = year2024Count,
                    Year2025 = year2025Count,
                    Year = "2026",
                    ForecastedCount = yearlyForecast,
                    DiffTo2025 = diff,
                    GrowthRate = growthRate
                });
            }

            return View(forecasts);
        }
        public IActionResult PaymentMethodForecast()
        {
            var startDate = new DateTime(2025, 1, 1);
            var endDate = new DateTime(2025, 12, 31);

            // 1️⃣ Veriyi aylık ve ödeme yöntemi bazında grupla
            var monthlyPaymentData = _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .AsEnumerable()
                .GroupBy(o => new
                {
                    Month = new DateTime(o.OrderDate.Year, o.OrderDate.Month, 1),
                    o.PaymentMethod
                })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    PaymentMethod = g.Key.PaymentMethod,
                    OrderCount = g.Count()
                })
                .OrderBy(x => x.Month)
                .ToList();

            var forecasts = new List<object>();

            foreach (var method in monthlyPaymentData.Select(x => x.PaymentMethod).Distinct())
            {
                var methodData = monthlyPaymentData
                    .Where(x => x.PaymentMethod == method)
                    .Select((x, index) => new PaymentForecastData
                    {
                        PaymentMethod = method,
                        MonthIndex = index + 1,
                        OrderCount = x.OrderCount
                    })
                    .ToList();

                // 2️⃣ Minimum veri kontrolü: SSA için en az 6 veri olmalı
                if (methodData.Count < 6)
                    continue;

                // 3️⃣ Dinamik windowSize ayarı (trainSize > 2 * windowSize kuralı sağlanıyor)
                int defaultWindowSize = 12;
                int windowSize = Math.Min(defaultWindowSize, (methodData.Count - 1) / 2);

                if (windowSize < 2)
                    continue; // Çok az veri varsa atla

                var dataView = _mlContext.Data.LoadFromEnumerable(methodData);

                // 4️⃣ SSA pipeline
                var pipeline = _mlContext.Forecasting.ForecastBySsa(
                    outputColumnName: "ForecastedValues",
                    inputColumnName: nameof(PaymentForecastData.OrderCount),
                    windowSize: windowSize,
                    seriesLength: methodData.Count,
                    trainSize: methodData.Count,
                    horizon: 12,
                    confidenceLevel: 0.95f
                );

                var model = pipeline.Fit(dataView);
                var engine = model.CreateTimeSeriesEngine<PaymentForecastData, PaymentForecastPrediction>(_mlContext);
                var prediction = engine.Predict();

                // 5️⃣ Tahmin sonuçlarını 2026 yılı aylarına ekle
                for (int i = 0; i < prediction.ForecastedValues.Length; i++)
                {
                    forecasts.Add(new
                    {
                        PaymentMethod = method,
                        Month = new DateTime(2026, i + 1, 1).ToString("yyyy MMMM"),
                        ForeCastCount = (int)prediction.ForecastedValues[i]
                    });
                }
            }

            return View(forecasts);
        }
    }
}

