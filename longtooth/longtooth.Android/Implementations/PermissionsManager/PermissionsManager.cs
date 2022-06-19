using Java.Lang;
using longtooth.Abstractions.DTOs;
using longtooth.Abstractions.Interfaces.Permissions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using static Xamarin.Essentials.Permissions;

namespace longtooth.Droid.Implementations.PermissionsManager
{
    public class PermissionsManager : IPermissionsManager
    {
        public async Task<bool> RequestPermissionAsync<T>() where T : BasePermission, new()
        {
            var status = await CheckStatusAsync<T>();

            if (status == PermissionStatus.Granted)
            {
                return true;
            }

            var requestResult = await RequestAsync<T>();

            return requestResult == PermissionStatus.Granted;
        }

        public RootRequestResultDto RequestRootAccess()
        {
            var suProcess = Runtime.GetRuntime().Exec("su");

            var outputStream = new Java.IO.DataOutputStream(suProcess.OutputStream);
            var inputStream = new Java.IO.DataInputStream(suProcess.InputStream);

            if (outputStream == null || inputStream == null)
            {
                // Looks like not rooted
                return new RootRequestResultDto(false, false);
            }

            // Asking for root
            outputStream.WriteBytes("id\n");
            outputStream.Flush();
            var currentUserId = inputStream.ReadLine();

            RootRequestResultDto result;
            var exitSu = false;

            if (currentUserId == null)
            {
                // Can't get root
                result = new RootRequestResultDto(false, false);
                exitSu = false;
            }
            else if (currentUserId.Contains("uid=0"))
            {
                // Got root
                result = new RootRequestResultDto(true, true);
                exitSu = true;
            }
            else
            {
                // Root access rejected by user
                result = new RootRequestResultDto(true, false);
                exitSu = true;
            }

            if (exitSu)
            {
                outputStream.WriteBytes("exit\n");
                outputStream.Flush();
            }

            return result;
        }
    }
}