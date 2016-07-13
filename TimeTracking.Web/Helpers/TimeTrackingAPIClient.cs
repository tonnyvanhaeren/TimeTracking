using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace TimeTracking.Web.Helpers
{
    public static class TimeTrackingAPIClient
    {
        public static HttpClient GetClient(ClaimsPrincipal cp, Boolean setToken = false)
        {
            string token = "";
            HttpClient client = new HttpClient();

            if (setToken) { 
                token = cp.FindFirst("access_token")?.Value;
                client.SetBearerToken(token);
            }


            client.BaseAddress = new Uri(General.Constants.ApiClient.ApiUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }
}
