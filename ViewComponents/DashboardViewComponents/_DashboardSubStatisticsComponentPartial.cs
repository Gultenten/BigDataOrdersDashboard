using BigDataOrdersDashboard.Context;
using Microsoft.AspNetCore.Mvc;
using System.Linq;


namespace BigDataOrdersDashboard.ViewComponents.DashboardViewComponents
{
    public class _DashboardSubStatisticsComponentPartial:ViewComponent
    {
        private readonly BigDataOrderDbContext _context;

        public _DashboardSubStatisticsComponentPartial(BigDataOrderDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {

            ViewBag.CategoryCount= _context.Categories.Count();
            ViewBag.CustomerCount= _context.Customers.Count();
            ViewBag.ProductCount= _context.Products.Count();
            ViewBag.OrderCount= _context.Orders.Count();
            ViewBag.CountryCount = _context.Customers.Select(x => x.CustomerCountry).Distinct().Count();
            ViewBag.CityCount = _context.Customers.Select(x => x.CustomerCity).Distinct().Count();
            return View();
        }
    }
}
