using Microsoft.AspNetCore.Mvc;

namespace TimeTracking.IdSrv.Configuration.UI.Home
{
    public class HomeController : Controller
    {
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }
    }
}