using System.Threading.Tasks;

namespace TimeTracking.Web.Services.Interfaces
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
