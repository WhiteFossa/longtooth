using System.Threading.Tasks;

namespace longtooth.Mobile.Abstractions.Interfaces.UserNotifier
{
    /// <summary>
    /// Interface to show alerts and notifications to an user
    /// </summary>
    public interface IUserNotifier
    {
        /// <summary>
        /// Show error message to an user
        /// </summary>
        Task ShowErrorMessageAsync(string title, string message);

        /// <summary>
        /// Show notification message to user
        /// </summary>
        Task ShowNotificationMessageAsync(string title, string message);

        /// <summary>
        /// Show a message with Yes and No buttons. Returns true if user chooses Yes
        /// </summary>
        Task<bool> ShowYesNoRequestAsync(string title, string message);
    }
}
