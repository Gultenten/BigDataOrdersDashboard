using BigDataOrdersDashboard.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BigDataOrdersDashboard.ViewComponents.DashboardViewComponents
{
    public class _DashboardLowStockProductsComponenPartial:ViewComponent
    {
        private readonly BigDataOrderDbContext _context;

        public _DashboardLowStockProductsComponenPartial(BigDataOrderDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var values = _context.Products.Include(x => x.Category).Where(y => y.StockQuantity <= 9).Take(15).ToList();


            return View(values);
        }


    }
}
