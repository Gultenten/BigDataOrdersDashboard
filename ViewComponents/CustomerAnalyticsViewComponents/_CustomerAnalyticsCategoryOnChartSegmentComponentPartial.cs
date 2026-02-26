using BigDataOrdersDashboard.Context;
using BigDataOrdersDashboard.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BigDataOrdersDashboard.ViewComponents.CustomerAnalyticsViewComponents
{
    
    public class _CustomerAnalyticsCategoryOnChartSegmentComponentPartial: ViewComponent
    {
        private readonly BigDataOrderDbContext _context;

        public _CustomerAnalyticsCategoryOnChartSegmentComponentPartial(BigDataOrderDbContext context)
        {
            _context = context;
        }

        //public IViewComponentResult Invoke()
        //{


        //    #region chart
        //    //Tüm siparişleri kategori adı ve yıla göre gruplama işlemi
        //    var categoryData = _context.Orders
        //        .Include(o => o.Product)
        //        .ThenInclude(p => p.Category)
        //        .AsEnumerable()
        //        .GroupBy(o => new
        //        {
        //            Year = o.OrderDate.Year,
        //            CategoryName = o.Product.Category.CategoryName

        //        })
        //        .Select(g => new
        //        {
        //            Year = g.Key.Year,
        //            CategoryName = g.Key.CategoryName,
        //            OrderCount = g.Count()
        //        })
        //        .OrderBy(x => x.Year)
        //        .ToList();



        //    //Her bir kategorinin toplam sipariş sayısı

        //    var topCategories = categoryData
        //         .GroupBy(x => x.CategoryName)
        //         .Select(g => new
        //         {
        //             CategoryName = g.Key,
        //             TotalOrders = g.Sum(x => x.OrderCount)
        //         })
        //         .OrderByDescending(x => x.TotalOrders)
        //         .Take(5)
        //         .Select(x => x.CategoryName)
        //         .ToList();


        //    //sadece bu beş kategoriye ait verilerin lisyesi
        //    var filteredData = categoryData
        //       .Where(x => topCategories.Contains(x.CategoryName)).ToList();

        //    var years = filteredData.Select(x => x.Year).Distinct().OrderBy(x => x).ToList();

        //    var chartSeries = topCategories.Select(category => new
        //    {
        //        name = category,
        //        data = years.Select(y => filteredData.FirstOrDefault(cd => cd.CategoryName == category && cd.Year == y)?.OrderCount ?? 0).ToList()
        //    }).ToList();

        //    ViewBag.Years = years;
        //    ViewBag.Series = chartSeries;


        //    #endregion

        //    #region statistics
        //    var today = DateTime.Today;
        //    var topCategoriesToday = _context.Orders
        //        .Include(o => o.Product)
        //        .ThenInclude(p => p.Category)
        //        .Where(x => x.OrderDate.Date == today)
        //        .AsEnumerable()
        //        .GroupBy(o => o.Product.Category.CategoryName)
        //        .Select(g => new
        //        {
        //            CategoryName = g.Key,
        //            OrderCount = g.Count()
        //        }).OrderByDescending(x => x.OrderCount)
        //        .Take(3)
        //        .ToList();

        //    if (topCategoriesToday.Count > 0)
        //    {
        //        ViewBag.TopCategory1Name = topCategoriesToday[0].CategoryName;
        //        ViewBag.TopCategory1Count = topCategoriesToday[0].OrderCount;
        //    }

        //    if (topCategoriesToday.Count > 1)
        //    {
        //        ViewBag.TopCategory2Name = topCategoriesToday[1].CategoryName;
        //        ViewBag.TopCategory2Count = topCategoriesToday[1].OrderCount;
        //    }

        //    if (topCategoriesToday.Count > 2)
        //    {
        //        ViewBag.TopCategory3Name = topCategoriesToday[2].CategoryName;
        //        ViewBag.TopCategory3Count = topCategoriesToday[2].OrderCount;
        //    }


        //    #endregion 


        //    return View();
        //}
        public IViewComponentResult Invoke()
        {
            #region chart
            var categoryData = _context.Orders
                .Where(o => o.Product != null && o.Product.Category != null)
                .Select(o => new
                {
                    Year = o.OrderDate.Year,
                    CategoryName = o.Product.Category.CategoryName ?? "Bilinmiyor"
                })
                .ToList() // buradan sonra bellekte
                .GroupBy(o => new { o.Year, o.CategoryName })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.CategoryName,
                    OrderCount = g.Count()
                })
                .OrderBy(x => x.Year)
                .ToList();

            var topCategories = categoryData
                .GroupBy(x => x.CategoryName)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    TotalOrders = g.Sum(x => x.OrderCount)
                })
                .OrderByDescending(x => x.TotalOrders)
                .Take(5)
                .Select(x => x.CategoryName)
                .ToList();

            var filteredData = categoryData
                .Where(x => topCategories.Contains(x.CategoryName)).ToList();

            var years = filteredData.Select(x => x.Year).Distinct().OrderBy(x => x).ToList();

            var chartSeries = topCategories.Select(category => new
            {
                name = category,
                data = years.Select(y => filteredData
                    .FirstOrDefault(cd => cd.CategoryName == category && cd.Year == y)?.OrderCount ?? 0)
                    .ToList()
            }).ToList();

            ViewBag.Years = years;
            ViewBag.Series = chartSeries;
            #endregion

            #region statistics
            var today = DateTime.Today;

            var topCategoriesToday = _context.Orders
                .Where(x => x.OrderDate.Date == today && x.Product != null && x.Product.Category != null)
                .Select(o => new
                {
                    CategoryName = o.Product.Category.CategoryName ?? "Bilinmiyor"
                })
                .ToList() // bellekte
                .GroupBy(o => o.CategoryName)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    OrderCount = g.Count()
                })
                .OrderByDescending(x => x.OrderCount)
                .Take(3)
                .ToList();

            if (topCategoriesToday.Count > 0)
            {
                ViewBag.TopCategory1Name = topCategoriesToday[0].CategoryName;
                ViewBag.TopCategory1Count = topCategoriesToday[0].OrderCount;
            }
            if (topCategoriesToday.Count > 1)
            {
                ViewBag.TopCategory2Name = topCategoriesToday[1].CategoryName;
                ViewBag.TopCategory2Count = topCategoriesToday[1].OrderCount;
            }
            if (topCategoriesToday.Count > 2)
            {
                ViewBag.TopCategory3Name = topCategoriesToday[2].CategoryName;
                ViewBag.TopCategory3Count = topCategoriesToday[2].OrderCount;
            }
            #endregion

            return View();
        }
    }
    
}
