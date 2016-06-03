using System.Threading.Tasks;

namespace TimeTracking.IdSrv.Services.Interfaces
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
