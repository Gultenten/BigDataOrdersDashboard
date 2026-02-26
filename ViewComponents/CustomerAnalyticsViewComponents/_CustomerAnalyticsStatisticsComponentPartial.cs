using BigDataOrdersDashboard.Context;
using Microsoft.AspNetCore.Mvc;

namespace BigDataOrdersDashboard.ViewComponents.CustomerAnalyticsViewComponents
{
    public class _CustomerAnalyticsStatisticsComponentPartial:ViewComponent
    {
        private readonly BigDataOrderDbContext _context;

        public _CustomerAnalyticsStatisticsComponentPartial(BigDataOrderDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            ViewBag.TopCustomer = _context.Orders.GroupBy(o => new { o.Customer.CustomerName, o.Customer.CustomerSurname }).Select(g => new
            {
                FullName = g.Key.CustomerName + " " + g.Key.CustomerSurname,
                TotalOrders = g.Count()
            }).OrderByDescending(x => x.TotalOrders).Select(x => x.FullName).FirstOrDefault();


            ViewBag.TopCity = _context.Orders.GroupBy(o => o.Customer.CustomerCity).Select(g => new
            {
                Country = g.Key,
                TotalOrders = g.Count()
            })
         .OrderByDescending(x => x.TotalOrders)
         .Select(x => x.Country)
         .FirstOrDefault();


            ViewBag.Last30DaysOrderCount = _context.Orders.Where(o => o.OrderDate>=DateTime.Now.AddDays(-30)).Select(o => o.CustomerId).Distinct().Count();

            ViewBag.TopPaymentMethod = _context.Orders.GroupBy(x => x.PaymentMethod)
             .Select(y => new
             {
                 PaymentMethod = y.Key,
                 TotalOrders = y.Count()
             }).OrderByDescending(z => z.TotalOrders).Select(x => x.PaymentMethod).FirstOrDefault();


            return View();
        }
    }
}
