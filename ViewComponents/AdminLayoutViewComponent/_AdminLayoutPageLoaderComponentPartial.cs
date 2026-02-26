using Microsoft.AspNetCore.Mvc;

namespace BigDataOrdersDashboard.ViewComponents.AdminLayoutViewComponent
{
    public class _AdminLayoutPageLoaderComponentPartial:ViewComponent
    {public IViewComponentResult Invoke()
        {
            {
                return View();
            }
        }
    }
}
