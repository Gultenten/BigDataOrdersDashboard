using BigDataOrdersDashboard.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BigDataOrdersDashboard.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly BigDataOrderDbContext _context;

        public StatisticsController(BigDataOrderDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.CategoryCount = _context.Categories.Count();
            ViewBag.CustomerCount = _context.Customers.Count();
            ViewBag.ProductCount = _context.Products.Count();
            ViewBag.OrderCount = _context.Orders.Count();


            ViewBag.CustomerCount = _context.Customers.Select(y => y.CustomerCountry).Distinct().Count();
            ViewBag.CustomerCity = _context.Customers.Select(y => y.CustomerCity).Distinct().Count();
            ViewBag.OrderStatusByCompleted = _context.Orders.Where(y => y.OrderStatus == "Tamamlandı").Count();
            ViewBag.OrderStatusByCancelled = _context.Orders.Where(y => y.OrderStatus == "İptal Edildi").Count();


            ViewBag.OctobersOrder = _context.Orders.FromSqlRaw("select * from Orders where OrderDate>='2025-10-01' and OrderDate<'2025-11-01'").Count();
            ViewBag.Orders2025Count = _context.Orders.Where(x => x.OrderDate.Year == 2025).Count();

            ViewBag.AverageProductPrice = _context.Products.Average(x => x.UnitPrice);
            ViewBag.AverageProductQuantity = _context.Products.Average(x => x.StockQuantity);


            return View();
        }

        //public IActionResult TestualStatistics()
        //{

        //    ViewBag.MostExpensiveProduct = _context.Products.Where(x => x.UnitPrice == (_context.Products.Max(x => x.UnitPrice))).Select(y => y.ProductName).FirstOrDefault();

        //    ViewBag.CheapestProduct = _context.Products.Where(x => x.UnitPrice == (_context.Products.Min(c => c.UnitPrice))).Select(y => y.ProductName).FirstOrDefault();


        //    ViewBag.TopStockProduct = _context.Products.OrderByDescending(x => x.StockQuantity).Take(1).Select(y => y.ProductName).FirstOrDefault();


        //    ViewBag.LastAddedProduct = _context.Products.OrderByDescending(x => x.ProductId).Take(1).Select(y => y.ProductName).FirstOrDefault();


        //    ViewBag.LastAddedCustomer = _context.Customers.OrderByDescending(x => x.CustomerId).Take(1).Select(y => y.CustomerName + " " + y.CustomerSurname).FirstOrDefault();

        //    ViewBag.TopPaymentMethod = _context.Orders.GroupBy(x => x.PaymentMethod)
        //        .Select(y => new
        //        {
        //            PaymentMethod = y.Key,
        //            TotalOrders = y.Count()
        //        }).OrderByDescending(z => z.TotalOrders).Select(x => x.PaymentMethod).FirstOrDefault();


        //    ViewBag.TopOrderedProduct = _context.Orders.GroupBy(o => o.Product.ProductName).Select(g => new
        //    {
        //        ProductName = g.Key,
        //        TotalQuantity = g.Sum(x => x.Quantity)
        //    }).OrderByDescending(r => r.TotalQuantity).Select(y => y.ProductName).FirstOrDefault();

        //    ViewBag.MinOrderedProduct = _context.Orders.GroupBy(o => o.Product.ProductName).Select(g => new
        //    {
        //        ProductName = g.Key,
        //        TotalQuantity = g.Sum(x => x.Quantity)
        //    }).OrderBy(r => r.TotalQuantity).Select(y => y.ProductName).FirstOrDefault();

        //    ViewBag.TopCountry = _context.Orders.GroupBy(o => o.Customer.CustomerCountry).Select(g => new
        //    {
        //        Country = g.Key,
        //        TotalOrders = g.Count()
        //    })
        //    .OrderByDescending(x => x.TotalOrders)
        //    .Select(x => x.Country)
        //    .FirstOrDefault();


        //    ViewBag.TopCity = _context.Orders.GroupBy(o => o.Customer.CustomerCity).Select(g => new
        //    {
        //        Country = g.Key,
        //        TotalOrders = g.Count()
        //    })
        //    .OrderByDescending(x => x.TotalOrders)
        //    .Select(x => x.Country)
        //    .FirstOrDefault();

        //    ViewBag.TopCategory = _context.Orders.GroupBy(o => o.Product.Category.CategoryName)
        //    .Select(g => new
        //    {
        //        CategoryName = g.Key,
        //        TotalSales = g.Sum(x => x.Quantity)
        //    }).OrderByDescending(x => x.TotalSales).Select(x => x.CategoryName).FirstOrDefault();


        //    ViewBag.TopCustomer = _context.Orders.GroupBy(o => new { o.Customer.CustomerName,o.Customer.CustomerSurname}).Select(g => new
        //    {
        //        FullName = g.Key.CustomerName+" "+g.Key.CustomerSurname,
        //        TotalOrders = g.Count()
        //    }).OrderByDescending(x => x.TotalOrders).Select(x => x.FullName).FirstOrDefault();



        //    ViewBag.MostCompletedProduct = _context.Orders.Where(o => o.OrderStatus == "Tamamlandı").GroupBy(o => o.Product.ProductName)
        //   .Select(g => new
        //   {
        //       ProductName = g.Key,
        //       CompletedOrders = g.Count()
        //   }).OrderByDescending(x => x.CompletedOrders).Select(x => x.ProductName).FirstOrDefault();

        //    ViewBag.TopReturnedProduct = _context.Orders.Where(o => o.OrderStatus == "İptal Edildi").GroupBy(o => o.Product.ProductName)
        //   .Select(g => new
        //   {
        //       ProductName = g.Key,
        //       CompletedOrders = g.Count()
        //   }).OrderByDescending(x => x.CompletedOrders).Select(x => x.ProductName).FirstOrDefault();


        //    ViewBag.LowestStockProduct= _context.Products.OrderBy(x => x.StockQuantity).Take(1).Select(y => y.ProductName).FirstOrDefault();

        //    ViewBag.LowestActiveCategory= _context.Orders.GroupBy(o => o.Product.Category.CategoryName)
        //    .Select(g => new
        //    {
        //        CategoryName = g.Key,
        //        TotalSales = g.Sum(x => x.Quantity)
        //    }).OrderBy(x => x.TotalSales).Select(x => x.CategoryName).FirstOrDefault();
        //    return View();
        //}

        public IActionResult TestualStatistics()
        {
            // Ürün verilerini tek sorguda çek
            var products = _context.Products
                .Select(p => new { p.ProductName, p.UnitPrice, p.StockQuantity, p.ProductId })
                .ToList();

            // Sipariş verilerini tek sorguda çek
            //var orders = _context.Orders
            //    .Select(o => new
            //    {
            //        o.Quantity,
            //        o.PaymentMethod,
            //        o.OrderStatus,
            //        ProductName = o.Product.ProductName,
            //        CategoryName = o.Product.Category.CategoryName,
            //        o.Customer.CustomerName,
            //        o.Customer.CustomerSurname,
            //        o.Customer.CustomerCountry,
            //        o.Customer.CustomerCity
            //    })
            //    .ToList();
            var orders = _context.Orders
    .Select(o => new
    {
        o.Quantity,
        PaymentMethod = o.PaymentMethod ?? "",
        OrderStatus = o.OrderStatus ?? "",
        ProductName = o.Product.ProductName ?? "",
        CategoryName = o.Product.Category.CategoryName ?? "",
        CustomerName = o.Customer.CustomerName ?? "",
        CustomerSurname = o.Customer.CustomerSurname ?? "",
        CustomerCountry = o.Customer.CustomerCountry ?? "",
        CustomerCity = o.Customer.CustomerCity ?? ""
    })
    .ToList();

            // --- ÜRÜN VIEWBAGLERİ (bellekte hesapla) ---
            var maxPrice = products.Max(x => x.UnitPrice);
            var minPrice = products.Min(x => x.UnitPrice);

            ViewBag.MostExpensiveProduct = products
                .Where(x => x.UnitPrice == maxPrice)
                .Select(x => x.ProductName)
                .FirstOrDefault();

            ViewBag.CheapestProduct = products
                .Where(x => x.UnitPrice == minPrice)
                .Select(x => x.ProductName)
                .FirstOrDefault();

            ViewBag.TopStockProduct = products
                .OrderByDescending(x => x.StockQuantity)
                .Select(x => x.ProductName)
                .FirstOrDefault();

            ViewBag.LowestStockProduct = products
                .OrderBy(x => x.StockQuantity)
                .Select(x => x.ProductName)
                .FirstOrDefault();

            ViewBag.LastAddedProduct = products
                .OrderByDescending(x => x.ProductId)
                .Select(x => x.ProductName)
                .FirstOrDefault();

            // --- MÜŞTERİ VIEWBAGLERİ ---
            ViewBag.LastAddedCustomer = _context.Customers
                .OrderByDescending(x => x.CustomerId)
                .Select(y => y.CustomerName + " " + y.CustomerSurname)
                .FirstOrDefault();

            // --- SİPARİŞ VIEWBAGLERİ (bellekte hesapla) ---
            ViewBag.TopPaymentMethod = orders
                .GroupBy(x => x.PaymentMethod)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            ViewBag.TopOrderedProduct = orders
                .GroupBy(o => o.ProductName)
                .OrderByDescending(g => g.Sum(x => x.Quantity))
                .Select(g => g.Key)
                .FirstOrDefault();

            ViewBag.MinOrderedProduct = orders
                .GroupBy(o => o.ProductName)
                .OrderBy(g => g.Sum(x => x.Quantity))
                .Select(g => g.Key)
                .FirstOrDefault();

            ViewBag.TopCountry = orders
                .GroupBy(o => o.CustomerCountry)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            ViewBag.TopCity = orders
                .GroupBy(o => o.CustomerCity)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            ViewBag.TopCategory = orders
                .GroupBy(o => o.CategoryName)
                .OrderByDescending(g => g.Sum(x => x.Quantity))
                .Select(g => g.Key)
                .FirstOrDefault();

            ViewBag.LowestActiveCategory = orders
                .GroupBy(o => o.CategoryName)
                .OrderBy(g => g.Sum(x => x.Quantity))
                .Select(g => g.Key)
                .FirstOrDefault();

            ViewBag.TopCustomer = orders
                .GroupBy(o => o.CustomerName + " " + o.CustomerSurname)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            ViewBag.MostCompletedProduct = orders
                .Where(o => o.OrderStatus == "Tamamlandı")
                .GroupBy(o => o.ProductName)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            ViewBag.TopReturnedProduct = orders
                .Where(o => o.OrderStatus == "İptal Edildi")
                .GroupBy(o => o.ProductName)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            return View();
        }
    }
}
