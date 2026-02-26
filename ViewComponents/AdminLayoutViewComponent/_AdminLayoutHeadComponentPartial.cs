using Microsoft.AspNetCore.Mvc;

namespace BigDataOrdersDashboard.ViewComponents.AdminLayoutViewComponent
{
    public class _AdminLayoutHeadComponentPartial:ViewComponent
    {
        public IViewComponentResult  Invoke()
        {
            return View();
        }
    }
}
