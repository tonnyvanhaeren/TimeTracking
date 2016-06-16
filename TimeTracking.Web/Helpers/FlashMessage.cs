using MotleyFlash;
using MotleyFlash.Extensions;

namespace TimeTracking.Web.Helpers
{
    public class FlashMessage
    {
        private readonly IMessenger _messenger;

        public FlashMessage(IMessenger messenger)
        {
            _messenger = messenger;
        }

        public void ShowSuccessMessage(string msg, string title)
        {
            _messenger.Success(msg, title);
        }

        public void ShowErrorMessage(string msg, string title)
        {
            _messenger.Error(msg, title);
        }

        public void ShowNoticeMessage(string msg, string title)
        {
            _messenger.Notice(msg, title);
        }

        public void ShowInfoMessage(string msg, string title)
        {
            _messenger.Information(msg, title);
        }

        public void ShowWarningMessage(string msg, string title)
        {
            _messenger.Warning(msg, title);
        }
    }
}
