using Microsoft.AspNetCore.Mvc;

namespace Ventixe.MVC.Controllers
{
    public class EventController (): Controller
    {
        [Route("Home/events")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Events";
            return View();
        }
    }
}
