using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Microsoft.DotNet.Cli.Utils;

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

        public async Task<IActionResult> Test()
        {
            var cp = (ClaimsPrincipal)User;
            var token = cp.FindFirst("access_token")?.Value;

            var client = new HttpClient();
            client.SetBearerToken(token);

            var response = await client.GetStringAsync(General.Constants.ApiClient.ApiUrlIdentityEndPoint);
            ViewBag.Json = JArray.Parse(response).ToString();

            return View();
        }
    }
}
