using BigDataOrdersDashboard.Context;
using Microsoft.AspNetCore.Mvc;

namespace BigDataOrdersDashboard.ViewComponents.CustomerAnalyticsViewComponents
{
    public class _CustomerAnalyticsMainStatisticsComponentPartial:ViewComponent
    {
        private readonly BigDataOrderDbContext _contetx;

        public _CustomerAnalyticsMainStatisticsComponentPartial(BigDataOrderDbContext contetx)
        {
            _contetx = contetx;
        }

        public IViewComponentResult Invoke()
        {

            var totalCustomerCount= _contetx.Customers.Count();
            ViewBag.TotalCustomerCount = totalCustomerCount;

            var totalOrderCount = _contetx.Orders.Count();

            var averageOrderPerCustomerCount = totalOrderCount / totalCustomerCount;

            ViewBag.AverageOrderPerCustomerCount = averageOrderPerCustomerCount;

            var threeMonthsAgo = DateTime.Now.AddMonths(-3);

            var activeCustomerCount = _contetx.Orders.Where(o => o.OrderDate >= threeMonthsAgo).Select(o => o.CustomerId).Distinct().Count();


            ViewBag.ActiveCustomerCount = activeCustomerCount;

            var sixMonthAgo= DateTime.Now.AddMonths(-6);
            var inactiveCustomerCount = _contetx.Customers.Count(c => !_contetx.Orders.Any(o => o.CustomerId == c.CustomerId && o.OrderDate>=sixMonthAgo));

            ViewBag.InactiveCustomerCount = inactiveCustomerCount;



            {
                return View();
            }
        }
    }
}
