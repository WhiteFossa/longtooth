using Acr.UserDialogs;
using longtooth.Mobile.Abstractions.Interfaces.UserNotifier;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace longtooth.Droid.Implementations.UserNotifier
{
    public class UserNotifier : IUserNotifier
    {
        public async Task ShowErrorMessageAsync(string title, string message)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await UserDialogs.Instance.AlertAsync(message, title, "OK");
            });
        }

        public async Task ShowNotificationMessageAsync(string title, string message)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await UserDialogs.Instance.AlertAsync(message, title, "OK");
            });
        }

        public async Task<bool> ShowYesNoRequestAsync(string title, string message)
        {
            // TODO: Is it possible to call it in main thread?
            return await UserDialogs.Instance.ConfirmAsync(message, title, "Yes", "No");
        }
    }
}