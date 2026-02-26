using BigDataOrdersDashboard.Context;
using BigDataOrdersDashboard.Dtos.LoyalthDtos;
using BigDataOrdersDashboard.Dtos.LoyalthMLDtos;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using System.Linq;

namespace BigDataOrdersDashboard.Controllers
{
    public class CustomerLoyalthController : Controller
    {
        private readonly BigDataOrderDbContext _context;
        private readonly string _modelPath = "wwwroot/mlmodels/LoyalthScoreModel.zip";

        public CustomerLoyalthController(BigDataOrderDbContext context)
        {
            _context = context;
        }

        public IActionResult ItalyLoyalthScore()
        {
            var loyalthScores = _context.Customers
                .Include(c => c.Orders)
                .ThenInclude(o => o.Product) 
                .Where(c => c.CustomerCountry == "İtalya" && c.CustomerCity=="Parma" || c.CustomerCity=="Bologna" ||
                c.CustomerCity == "Como" || c.CustomerCity == "Siena" || c.CustomerCity == "Verona" || c.CustomerCity == "Bergamo" || c.CustomerCity == "Bari" || c.CustomerCity == "Venedik")
                .Select(c => new
                {
                    CustomerName = c.CustomerName + " " + c.CustomerSurname,
                    TotalOrders = c.Orders.Count(),
                    //TotalSpent = c.Orders.Sum(o => o.Product.UnitPrice * o.Quantity),
                    TotalSpent = c.Orders
    .Sum(o => (o.Product.UnitPrice ?? 0m) * o.Quantity),
                    LastOrderDate = c.Orders.Max(o => (DateTime?)o.OrderDate)
                })
                .AsEnumerable()
                .Select(x =>
                {
                    var daysSinceLastOrder = (x.LastOrderDate.HasValue) ? (DateTime.Now - x.LastOrderDate.Value)
                    .TotalDays : double.MaxValue;

                    double recencyScore = daysSinceLastOrder switch
                    {
                        <= 30 => 100,
                        <= 90 => 80,
                        <= 180 => 50,
                        <= 365 => 25,
                        _ => 10
                    };

                    double frequencyScore = x.TotalOrders switch
                    {
                        >= 20 => 100,
                        >= 10 => 80,
                        >= 5 => 60,
                        >= 2 => 40,
                        1 => 20,
                        _ => 10
                    };

                    double monetaryScore = x.TotalSpent switch
                    {
                        >= 5000 => 100,
                        >= 3000 => 80,
                        >= 1000 => 60,
                        >= 500 => 40,
                        >= 100 => 20,
                        _ => 10
                    };

                    double loyalthScore = (recencyScore * 0.4) + (frequencyScore * 0.3) + (monetaryScore * 0.3);

                    return new LoyalthScoreDto
                    {
                        CustomerName = x.CustomerName,
                        TotalOrders = x.TotalOrders,
                       TotalSpent = (double)Math.Round(x.TotalSpent, 2),
                       
                        LastOrderDate = x.LastOrderDate,
                        LoyalthScore = Math.Round(loyalthScore, 2)
                    };


                }).OrderByDescending(x => x.LoyalthScore).ToList();






            return View(loyalthScores);
        }

        public IActionResult ItalyLoyalthScoreWithML()
        {
            //italyadaki belli şehirlerin sipariş listesi

            var data = _context.Customers
                .Include(c => c.Orders)
                .ThenInclude(o => o.Product)
                .Where(c => c.CustomerCountry == "İtalya" &&
                (c.CustomerCity == "Parma" ||
                c.CustomerCity == "Bologna" ||
                c.CustomerCity == "Como" ||
                c.CustomerCity == "Siena" ||
                c.CustomerCity == "Verona" ||
                c.CustomerCity == "Bergamo" ||
                c.CustomerCity == "Bari" ||
                c.CustomerCity == "Venedik"))
                .AsEnumerable()
                .Select(c =>
                {
                    //müşterinin son sipairş tarihini bul
                    var lastOrderDate = c.Orders.Max(o => (DateTime?)o.OrderDate);

                    //son siparişin üzerinden kaç gün geçtiğini hesaplama
                    var daySice = lastOrderDate.HasValue ? Math.Round((DateTime.Now - lastOrderDate.Value).TotalDays) : 999;

                    //rfm metrikleri

                    double recency = daySice;
                    double frequnecy = c.Orders.Count();
                    //double monetary = (double)Math.Round(c.Orders.Sum(o => o.Quantity * o.Product.UnitPrice),2);

                    double monetary = (double)Math.Round(c.Orders.Sum(o => o.Quantity * o.Product.UnitPrice).GetValueOrDefault(), 2);


                    //loyalyscore ağrlıklı ortalamanın bulunması
                    double loyalth = (RecencyScore(recency) * 0.4) +
                    (FrequencyScore(frequnecy) * 0.3) +
                    (MonetaryScore(monetary) * 0.3);


                    //ML.net e gidecek veri listesi
                    return new LoyalthMLDataScoreDto
                    {
                        CustomerName = c.CustomerName + " " + c.CustomerSurname,
                        Recency = (float)recency,
                        Frequency = (float)frequnecy,
                        Monetary = (float)monetary,
                        LoyalthScore = (float)loyalth

                    };

                }).ToList();



            //ML işlemleri

            var mlContext = new MLContext();
            IDataView dataView = mlContext.Data.LoadFromEnumerable(data);


            //pipeline

            /* var pipeline = mlContext.Transforms
                 .Concatenate(" Features", "Recency", "Frequency", "Monetary")
                 .Append(mlContext.Regression.Trainers.Sdca(
                     labelColumnName: "LoyalthScore",
                     maximumNumberOfIterations: 100));*/

            var pipeline = mlContext.Transforms
                   .Concatenate("Features", "Recency", "Frequency", "Monetary")
                   .Append(mlContext.Transforms.NormalizeMinMax("Features")) // 🔥 Ölçekleme eklendi
                   .Append(mlContext.Regression.Trainers.Sdca(
                       labelColumnName: "LoyalthScore",
                       maximumNumberOfIterations: 100));

            //modeli eğitme
            var model = pipeline.Fit(dataView);


            //modeli kaydetme
            mlContext.Model.Save(model, dataView.Schema, _modelPath);


            //tahmin metodu
            var predictionEngine = mlContext.Model.CreatePredictionEngine<LoyalthMLDataScoreDto
                ,LoyalthScoreMLPredictionDto>(model);


            //her müşteri için ML data tahmini
            var results = data.Select(x =>
            {
                var prediction = predictionEngine.Predict(new LoyalthMLDataScoreDto
                {
                    Recency = x.Recency,
                    Frequency = x.Frequency,
                    Monetary = x.Monetary,

                });

                return new ResultLoyalthScoreMLDto
                {
                    CustomerName = x.CustomerName,
                    Recency = x.Recency,
                    Frequency = x.Frequency,
                    Monetary = x.Monetary,
                    ActualLoyalthScore = Math.Round(x.LoyalthScore, 2),
                    PredictedLoyalthScore = Math.Round(prediction.LoyalthScore, 2)

                };
            }).OrderByDescending(x=>x.PredictedLoyalthScore).ToList();  

            return View(results);
        }


        //yardımcı skor metotlarının yazılması
        private static double RecencyScore(double days) => days switch
        {
            <= 30 => 100,
            <= 90 => 75,
            <= 180 => 50,
            <= 365 => 25,
            _ => 10

        };

        private static double FrequencyScore(double orders) => orders switch
        {
            >= 20 => 100,
            >= 10 => 80,
            >= 5 => 60,
            >= 2 => 40,
            1 => 20,
            _=>10

        };


        private static double MonetaryScore(double spent) => spent switch
        {
            >= 5000 => 100,
            >= 3000 => 80,
            >= 1000 => 60,
            >= 500 => 40,
            >= 100 => 20,
            _ => 10

        };

    }
}
