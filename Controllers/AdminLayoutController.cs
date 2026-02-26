using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;

namespace BigDataOrdersDashboard.Controllers
{
    public class AdminLayoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}













