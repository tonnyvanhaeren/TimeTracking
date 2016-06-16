using System.Threading.Tasks;

namespace TimeTracking.Web.Services.Interfaces
{
    public interface IMailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
