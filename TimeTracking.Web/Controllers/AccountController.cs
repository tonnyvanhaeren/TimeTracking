using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeTracking.DataAccess.Interfaces;
using TimeTracking.General;
using TimeTracking.General.Helpers;
using TimeTracking.General.Models;
using TimeTracking.Web.Helpers;
using TimeTracking.Web.Services.Interfaces;
using TimeTracking.Web.Views;
using TimeTracking.Web.Views.Account;

namespace TimeTracking.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly FlashMessage _flash;
        private readonly IMailSender _mailSender;
        private readonly IPostGreSqlService _service;
        private readonly ConfirmationToken _token;

        public AccountController(IMailSender mailSender, IPostGreSqlService service, ConfirmationToken token, FlashMessage flash)
        {
            _mailSender = mailSender;
            _service = service;
            _token = token;
            _flash = flash;
        }

        //[Authorize]
        public IActionResult Login()
        {
            return RedirectToAction("Index", "Home");
        }

        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            await HttpContext.Authentication.SignOutAsync("cookies");
            return Redirect(General.Constants.Idsrv.IdSrvLogOutUrl + $"?returnUrl={General.Constants.MvcClient.ClientUrlLogOffEndPoint}"); //go to indentity server logout
        }


        [AllowAnonymous]
        public IActionResult Forbidden()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult LogOffMsg()
        {
            //show flashmessage
            _flash.ShowInfoMessage("You are logged Out", "Log Off Information:");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult UserInfo()
        {
            var email = User.Claims.Where(c => c.Type == JwtClaimTypes.Email).Select(c => c.Value).SingleOrDefault();

            AppUser user = _service.GetAppUserByEmail(email);
            List<AppUserPolicy> policies = _service.GetAllAppUserPolicies(user.Subject);
            AppUserViewModelView vmv = new AppUserViewModelView(user, policies);

            return View(vmv);
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var email = User.Claims.Where(c => c.Type == JwtClaimTypes.Email).Select(c => c.Value).SingleOrDefault();

            AppUser user = _service.GetAppUserByEmail(email);
            List<AppUserPolicy> policies = _service.GetAllAppUserPolicies(user.Subject);
            AppUserViewModelView vmv = new AppUserViewModelView(user, policies);

            return View(vmv);
        }


        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            //if model state is valid and the email is unique
            AppUser checkMailUser = _service.GetAppUserByEmail(model.Email);

            if (ModelState.IsValid)
            {
                if (checkMailUser == null)
                {
                    //add and save user
                    AppUser us = new AppUser { FamilyName = model.FamilyName, GivenName = model.GivenName, Email = model.Email, Username = model.Email };
                    _service.AddAppUser(us, model.Password);

                    //Generate token lifetime 10 min ans send confirmation request
                    var code = _token.Generate("EMAIL", us);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { subject = us.Subject, code = code }, protocol: HttpContext.Request.Scheme);

                    var strNow = String.Format("{0:ddd, d MMM, yyyy 'at:' HH:mm}", DateTime.Now); //"{0:ddd, MMM d, yyyy}"  {0:d/M/yyyy HH:mm}

                    await _mailSender.SendEmailAsync(us.Email, "Confirm your account",
                            $@"<h2 style='color: blue;'>Confirm your account</h2></br>
                            <h3>{us.GivenName} {us.FamilyName}</h3></br>
                            <h4>Time of sending: { strNow } </h4></br>
                            <h3>Please confirm your account by clicking this link within 9 minutes: 
                            <a href='{callbackUrl}'>link</a></h3>");

                    //show flashmessage
                    _flash.ShowWarningMessage($"Hello {us.GivenName}, {us.FamilyName} you received a mail (please respond within 9 minutes).",
                                                  "Confirm your account:");
                    //redirect to home
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $" Email is already in use by: {checkMailUser.GivenName} - {checkMailUser.FamilyName}");
                    return View(model);
                }
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ConfirmEmail(string subject, string code)
        {
            if (subject == null || code == null)
            {
                return View("Error"); //request parameters nill
            }

            AppUser us = _service.GetAppUserBySubject(subject);

            if (us == null) //AppUser not found
            {
                return View("Error");
            }

            if (!_token.ValidateToken("EMAIL", code, us))
            {
                return View("Error"); //invalid token
            }


            // set field EmailConfirmed, Enabed = true for the user in database
            // add Policy employee to user
            us.EmailConfirmed = true;
            us.Enabled = true;
            _service.UpdateAppUser(us);
            _service.AddPolicyToAppUser(us, Constants.AppUserPolicyType.Role, "employee");

            //show flashmessage
            _flash.ShowSuccessMessage($"Hello {us.GivenName}, {us.FamilyName} Email confirmation Ok.",
                                          "Email Confirmation :");

            return RedirectToAction("Index", "Home");
        }


        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

    }
}
