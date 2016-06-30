using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TimeTracking.Web.Controllers
{

    [Authorize(Policy = "EmployeeOnly")]
    public class EmployeeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Test()
        {
            var cp = (ClaimsPrincipal)User;
            var token = cp.FindFirst("access_token")?.Value;



            return View();
        }
    }
}
