using Microsoft.AspNetCore.Mvc;

namespace Ventixe.MVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
