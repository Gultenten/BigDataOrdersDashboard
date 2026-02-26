using BigDataOrdersDashboard.Context;
using Microsoft.AspNetCore.Mvc;

namespace BigDataOrdersDashboard.ViewComponents.CustomerDetailViewComponents
{
    public class _CustomerDetailMainCoverDetailTableComponentPartial:ViewComponent
    {
        private readonly BigDataOrderDbContext _context;

        public _CustomerDetailMainCoverDetailTableComponentPartial(BigDataOrderDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var value = _context.Customers.Where(x=>x.CustomerId==8).FirstOrDefault();
            
             return View(value); 
        }
    }
}
