using Microsoft.AspNetCore.Mvc;

namespace corona.Controllers
{
    public class HomeController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
