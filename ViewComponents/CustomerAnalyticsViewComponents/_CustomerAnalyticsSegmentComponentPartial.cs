using BigDataOrdersDashboard.Context;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace BigDataOrdersDashboard.ViewComponents.CustomerAnalyticsViewComponents
{
    public class _CustomerAnalyticsSegmentComponentPartial:ViewComponent
    {
        private readonly BigDataOrderDbContext _context;

        public _CustomerAnalyticsSegmentComponentPartial(BigDataOrderDbContext context)
        {
            _context = context;
        }


        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
