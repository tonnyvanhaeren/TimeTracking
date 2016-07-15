using System.ComponentModel.DataAnnotations;

namespace TimeTracking.Web.Views.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
