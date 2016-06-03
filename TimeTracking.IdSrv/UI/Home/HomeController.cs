using Microsoft.AspNetCore.Mvc;

namespace TimeTracking.IdSrv.configuration.UI.Home
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