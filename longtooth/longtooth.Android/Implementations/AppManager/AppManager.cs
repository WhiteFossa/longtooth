using Android.App;
using longtooth.Abstractions.Interfaces.AppManager;

namespace longtooth.Droid.Implementations.AppManager
{
    public class AppManager : IAppManager
    {
        public void CloseApp()
        {
            var activity = MainActivity.Instance as Activity;
            activity.Finish();
            System.Diagnostics.Process.GetCurrentProcess().Kill(); // Dirty, but 100% sure to quit
        }
    }
}