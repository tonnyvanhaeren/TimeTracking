using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TimeTracking.IdSrv.UI.Register
{
    public class RegisterController : Controller
    {
     
        [HttpGet(TimeTracking.General.Constants.RoutePaths.Register, Name = "Register")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost(TimeTracking.General.Constants.RoutePaths.Register)]
        public IActionResult Index(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //save user
                //send confirmation mail
                //show flash message
                //redirect to home

            }

            //var vm = new LoginViewModel(model);
            return View(model);
        }

    }
}
