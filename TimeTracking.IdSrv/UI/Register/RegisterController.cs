using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TimeTracking.DataAccess.Interfaces;
using TimeTracking.General.Helpers;
using TimeTracking.General.Models;
using TimeTracking.IdSrv.configuration.UI.Home;
using TimeTracking.IdSrv.Helpers;
using TimeTracking.IdSrv.Services.Interfaces;

namespace TimeTracking.IdSrv.UI.Register
{
    public class RegisterController : Controller
    {
        private readonly IMailSender _mailSender;
        private readonly IPostGreSqlService _service;
        private readonly ConfirmationToken _token;
        private readonly FlashMessage _flash;

        public RegisterController(IMailSender mailSender, IPostGreSqlService service, ConfirmationToken token, FlashMessage flash)
        {
            _mailSender = mailSender;
            _service = service;
            _token = token;
            _flash = flash;
        }

        [HttpGet(TimeTracking.General.Constants.RoutePaths.Register, Name = "Register")]
        public IActionResult Index(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost(TimeTracking.General.Constants.RoutePaths.Register)]
        public async Task<IActionResult> Index(RegisterViewModel model, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            //if model state is valid and the email is unique
            User checkMailUser = _service.GetUserByEmail(model.Email);
            if (ModelState.IsValid)
            {
                if (checkMailUser == null) { 
                    //add and save user
                    User us = new User { FamilyName = model.FamilyName, GivenName = model.GivenName, Email = model.Email, Username = model.Email };
                    _service.AddUser(us, model.Password);

                    //Generate token lifetime 10 min ans send confirmation request
                    var code = _token.Generate("EMAIL", us);
                    var callbackUrl = Url.Action("ConfirmEmail", "Register", new { subject = us.Subject, code = code }, protocol: HttpContext.Request.Scheme);

                    var strNow = String.Format("{0:ddd, d MMM, yyyy at: HH:mm}", DateTime.Now); //"{0:ddd, MMM d, yyyy}"  {0:d/M/yyyy HH:mm}

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
                return View("Error");
            }

            User us = _service.GetUserBySubject(subject);

            if (us == null)
            {
                return View("Error");
            }

            if (!_token.ValidateToken("EMAIL", code, us))
            {
                return View("Error");
            }


            //set field EmailConfirmed = true for the user in database
            us.EmailConfirmed = true;
            us.Enabled = true;
            _service.UpdateUser(us);

            return View();
        }

        private bool CheckMailUniqueness(string email)
        {
            return _service.UserWithEmailIsUnique(email);
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
