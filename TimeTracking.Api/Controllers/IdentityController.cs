using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TimeTracking.Api.Controllers
{
    [Route("api/[controller]")]
    public class IdentityController : Controller
    {
           
        [HttpGet]
        public ActionResult Get()
        {
            //User.Claims;

            //return new JsonResult("okokok");
            return new JsonResult(User.Claims.Select(c => new { c.Type, c.Value }));
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
