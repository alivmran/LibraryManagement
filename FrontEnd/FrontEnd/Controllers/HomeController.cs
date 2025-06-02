using Microsoft.AspNetCore.Mvc;

namespace Library.Frontend.Controllers
{
    public class HomeController : Controller
    {
        // GET: / or /Home/Index
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Home/Privacy
        //public IActionResult Privacy()
        //{
        //    return View();
        //}
    }
}
