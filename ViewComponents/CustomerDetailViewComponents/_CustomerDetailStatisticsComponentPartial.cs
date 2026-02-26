using BigDataOrdersDashboard.Context;
using Microsoft.AspNetCore.Mvc;

namespace BigDataOrdersDashboard.ViewComponents.CustomerDetailViewComponents
{
    public class _CustomerDetailStatisticsComponentPartial:ViewComponent
    {
        public BigDataOrderDbContext _context;

        public _CustomerDetailStatisticsComponentPartial(BigDataOrderDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke(int id)
        {
            id = 8;
            ViewBag.TotalOrderCount = _context.Orders.Where(x => x.CustomerId == id).Count();

            ViewBag.CompletedOrderCount = _context.Orders.Where(x => x.CustomerId == id && x.OrderStatus=="Tamamlandı").Count();
            ViewBag.CancelledOrderCount = _context.Orders.Where(x => x.CustomerId == id && x.OrderStatus== "İptal Edildi").Count();

            ViewBag.GetCustomerCountry = _context.Customers.Where(x => x.CustomerId == id).Select(y=>y.CustomerCountry).FirstOrDefault();

            ViewBag.GetCustomerCity = _context.Customers.Where(x => x.CustomerId == id).Select(y => y.CustomerCity).FirstOrDefault();

          //  ViewBag.TotalSpentMoney = _context.Customers.Where(x => x.CustomerId == id).

            return View();
        }
    }
}
