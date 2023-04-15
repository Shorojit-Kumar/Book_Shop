using Microsoft.AspNetCore.Mvc;

namespace BookShop.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
