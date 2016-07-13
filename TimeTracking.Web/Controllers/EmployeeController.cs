using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using TimeTracking.Web.Helpers;
using TimeTracking.Web.Models;

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
            ClaimsPrincipal cp = User;
            HttpClient client = TimeTrackingAPIClient.GetClient(cp, true);

            //var response0 = await client.GetStringAsync(General.Constants.ApiClient.ApiUrlIdentityEndPoint);
            //var response = await client.GetStringAsync(General.Constants.ApiClient.ApiUrl + "api/values");

            HttpResponseMessage response = await client.GetAsync(General.Constants.ApiClient.ApiUrlIdentityEndPoint);

            if (response.IsSuccessStatusCode)
            {
                string ret = await response.Content.ReadAsStringAsync();
                ViewBag.Json = JArray.Parse(ret);
                return View();
            }
            else
            {
                ErrorView mv = new ErrorView();
                mv.Error = response.StatusCode.ToString();
                mv.Msg = $"Response From TimeTracking API => Access Denied for User : {cp.Identity.Name}";

                return View("Error", mv);
            }
        }
    }
}
