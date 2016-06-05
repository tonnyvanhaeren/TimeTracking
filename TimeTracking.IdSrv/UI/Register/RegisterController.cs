using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TimeTracking.DataAccess.Interfaces;
using TimeTracking.General.Models;
using TimeTracking.IdSrv.Services.Interfaces;

namespace TimeTracking.IdSrv.UI.Register
{
    public class RegisterController : Controller
    {
        private readonly IMailSender _mailSender;
        private readonly IPostGreSqlService _service;

        public RegisterController(IMailSender mailSender, IPostGreSqlService service)
        {
            _mailSender = mailSender;
            _service = service;
        }


        [HttpGet(TimeTracking.General.Constants.RoutePaths.Register, Name = "Register")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost(TimeTracking.General.Constants.RoutePaths.Register)]
        public async Task<IActionResult> Index(RegisterViewModel model)
        {
            bool checkMailUnique = CheckMailUniqueness(model.Email);

            //if model state is valid and the email is unique
            if (ModelState.IsValid && checkMailUnique)
            {
                //save user

                //send confirmation mail

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link

                //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var callbackUrl = Url.Action("ConfirmEmail", "Register", new { userId = 555, code = 77777 }, protocol: HttpContext.Request.Scheme);

                var email = "antonius.vanhaeren@telenet.be";
                var foreName = "tonny";
                var lastName = "Vanhaeren";

                await _mailSender.SendEmailAsync(email, "Confirm your account",
                    $"<h2 style='color: blue;'>Confirm your account</h2></br><h3>{foreName} {lastName}</h3></br><h3>Please confirm your account by clicking this link within 5 minutes: <a href='{callbackUrl}'>link</a></h3>");
                //show flash message

                //redirect to home
          

            }
            
            //if mail is not unique add error and return to view
            if (!checkMailUnique)
            {
                User user = _service.GetUserByEmail(model.Email);
                ModelState.AddModelError(string.Empty, $" Email is already in use by: {user.GivenName} - {user.FamilyName}");
                return View(model);
            }


            //var vm = new LoginViewModel(model);
            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }

            //var user = await _userManager.FindByIdAsync(userId);
            //if (user == null)
            //{
            //    return View("Error");
            //}
            //var result = await _userManager.ConfirmEmailAsync(user, code);
            return View();
        }

        private bool CheckMailUniqueness(string email)
        {
            return _service.UserWithEmailIsUnique(email);
        }

    }
}
